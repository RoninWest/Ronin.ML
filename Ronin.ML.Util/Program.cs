using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Ronin.ML.Text;
using Ronin.ML.Classifier;

namespace Ronin.ML.Util
{
	class Program : IDisposable
	{
		static void Main(params string[] args)
		{
			DateTime started = DateTime.UtcNow;

            if (args.Length < 2 || string.IsNullOrWhiteSpace(args.FirstOrDefault()) || string.IsNullOrWhiteSpace(args.LastOrDefault()))
                throw new ArgumentException(Usage);

            using (var logic = new Program())
            {
                switch (args.FirstOrDefault().ToLower())
                {
                    case "train":
                        logic.Train(args.LastOrDefault());
                        break;
                    case "classify":
                        logic.Classify(args.LastOrDefault());
                        break;
                    default:
                        throw new InvalidOperationException(Usage);
                }
            }

			Console.WriteLine("Took: {0}", DateTime.UtcNow - started);
#if DEBUG
			Console.WriteLine("\r\nPress ANY Key");
			Console.ReadKey();
#endif
		}

        static string Usage
        {
            get { return @"Usage:\r\ntrain path.txt\r\nclassify url"; }
        }

        readonly WordIndexGenerator _indexer;
        readonly WebTextExtractor _extractor;
        readonly IClassifier<string, string> _classifier;
        readonly IDataStorable _storableData;

		private Program()
		{
            Func<IWordProcessor, IWordProcessor> lCaseStem = n => new LengthFilter(
				new CaseNormalizer(
					new StemNormalizer(n)
				), 
			min:3);

			IWordProcessor stopWords = new StopWordFilter(lCaseStem(null), TextLanguage.Default);

			var ignoreFile = new FileInfo(Path.Combine(AssemblyDirectory, "WebWordIgnores.txt"));
			IWordProcessor ignores = new IgnoreWordFilter(stopWords, new WhiteSpaceTokenizer(), ignoreFile);
			IWordProcessor wp = lCaseStem(ignores);

            _indexer = new WordIndexGenerator(new NoneWordTokenizer(excludeNumber: true), wp);
            _extractor = new WebTextExtractor();

            var storage = new ClassifierDataInFile<string, string>();
            _storableData = storage;

            _classifier = new FisherClassifier<string, string, string>(storage, ExtractFeatures, GetThreshold);
            //string content = _extractor.Get();
            //WordIndex wi = _indexer.Process(content);
            //Print(wi);
		}

        public void Dispose()
        {
            if (_storableData != null)
                _storableData.Dispose();
        }

        IEnumerable<string> ExtractFeatures(string data)
        {
            WordIndex wi = _indexer.Process(data);
#if DEBUG
            //Print(wi);
#endif
            return wi.Keys;
        }

        static double GetThreshold(string cat)
        {
            switch (cat.ToLower())
            {
                case "seafood":
                    return .1;
                case "meat":
                    return .2;
                case "vegetarian":
                    return .3;
                case "vegan":
                    return .4;
                case "unknown":
                default:
                    return 0;
            }
        }

        public void Train(string path)
        {
            var f = new FileInfo(path);
            if (!f.Exists)
                throw new FileNotFoundException(path);

            using (FileStream fs = f.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(null);
                    string cat = arr.FirstOrDefault();
                    var url = new Uri(arr.LastOrDefault());
#if DEBUG
                    Console.WriteLine("{0}  | {1}", cat, url);
#endif
                    Task.Factory.StartNew(() =>
                    {
                        System.Threading.Thread.Sleep(100);
                        string content =_extractor.Get(url);
                        if(!string.IsNullOrWhiteSpace(content))
                            _classifier.ItemTrain(content, cat);
                    });
                }
            }
        }

        public void Classify(string url)
        {
            string content = _extractor.Get("http://food2fork.com");
#if DEBUG
            Console.WriteLine(content);
#endif
            System.Threading.Thread.Sleep(1000);
            content = _extractor.Get(url);
#if DEBUG
            Console.WriteLine(content);
#endif
            Classification<string> r = _classifier.ItemClassify(content, "unknown");
            Console.WriteLine("{0} {1:N0}", r.Category, r.Probability * 100);
        }

		static void Print(WordIndex wi)
		{
			if (wi == null)
				Console.WriteLine("<null>");
			else if (wi.Count == 0)
				Console.WriteLine("<empty>");
			else
			{
				var reOrder = from p in wi
							  orderby p.Value.Count descending, p.Key.Length descending, p.Value.First() ascending
							  select p;
				foreach (var p in reOrder.Take(200))
				{
					Console.Write("{0}:{1}, ", p.Key, p.Value.Count);
				}
				Console.WriteLine("\r\n====================\r\nTotal Words: {0:N0}", wi.Count);
			}
		}

		static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

	}
}
