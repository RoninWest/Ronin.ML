using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
            try
            {
                if (args.Length < 1 || string.IsNullOrWhiteSpace(args.FirstOrDefault()))
                    throw new ArgumentException(Usage);

                using (var logic = new Program())
                {
                    switch (args.FirstOrDefault().ToLower())
                    {
                        case "train":
                            if (args.Length < 2 || string.IsNullOrWhiteSpace(args.LastOrDefault()))
                                logic.Train();
                            else
                                logic.Train(args.LastOrDefault());
                            break;
                        case "classify":
                        case "test":
                            if (args.Length < 2 || string.IsNullOrWhiteSpace(args.LastOrDefault()))
                                throw new ArgumentException(Usage);

                            logic.Classify(args.LastOrDefault());
                            break;
                        default:
                            throw new InvalidOperationException(Usage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Took: {0}", DateTime.UtcNow - started);
#if DEBUG
                Console.WriteLine("\r\nPress ANY Key");
                Console.ReadKey();
#endif
            }
		}

        static string Usage
        {
            get { return @"Usage:\r\ntrain path.txt\r\nclassify url"; }
        }

        readonly WordIndexGenerator _indexer;
        readonly AsyncWebText _extractor;
        readonly IClassifier<string, string> _classifier;
        readonly IDataStorable _storableData;
        readonly IClassifierData<string, string> _storage;

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
            _extractor = new AsyncWebText();

            var storage = new ClassifierDataInFile<string, string>(false);
            storage.Load();
            _storage = storage;
            _storableData = storage;

            _classifier = new FisherClassifier<string, string, string>(storage, ExtractFeatures, GetThreshold);
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

        void RemovePreviousTraining()
        {
            var keys = _storage.CategoryKeys();
            if (keys != null)
            {
                foreach (string k in keys)
                {
                    _storage.RemoveCategory(k);
                }
                _storableData.Save();
            }
        }

        public void Train()
        {
            Train(Path.Combine(AssemblyDirectory, "RecipesURL.txt"));
        }

        public void Train(string path)
        {
            var f = new FileInfo(path);
            if (!f.Exists)
                throw new FileNotFoundException(path);

            RemovePreviousTraining();

            var tasks = new ConcurrentBag<Task>();
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
                    System.Threading.Thread.Sleep(100);
                    Console.WriteLine("{0}  | {1}", cat, url);
#endif
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        string content = _extractor.Get(url).GetAwaiter().GetResult();
                        if(!string.IsNullOrWhiteSpace(content))
                            _classifier.ItemTrain(content, cat);
                    }));
                }
            }

            Task.Factory.ContinueWhenAll(tasks.ToArray(), arr => 
            {
                Console.WriteLine("All {0} requests are done!", arr.Length);
            });
        }

        public void Classify(string url)
        {
            var u = new Uri(url);
            string content = _extractor.Get(u).GetAwaiter().GetResult();
#if DEBUG
            //Console.WriteLine(content);
#endif
            Classification<string> r = _classifier.ItemClassify(content, "unknown");
            Console.WriteLine("{0} => {1} {2:N0}%", u.PathAndQuery, r.Category, r.Probability * 100);
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
