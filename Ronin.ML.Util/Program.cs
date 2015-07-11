using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
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
                        case "test":
                            if (args.Length < 2 || string.IsNullOrWhiteSpace(args.LastOrDefault()))
                                logic.Test();
                            else
                                logic.Test(args.LastOrDefault());
                            break;
                        case "classify":
                            if (args.Length < 2 || string.IsNullOrWhiteSpace(args.LastOrDefault()))
                                throw new ArgumentException(Usage);

                            logic.Classify(args.Skip(1).ToArray());
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
				ConsoleKeyInfo k;
                do
                {
					Console.WriteLine("\r\nPress ESC Key to quit");
                    k = Console.ReadKey();
					while(k.KeyChar == '\0') //needed this clause to work with Mono console
					{
						Thread.Sleep(200);
						k = Console.ReadKey();
					}
                }
                while (k.Key != ConsoleKey.Escape);
#endif
            }
		}

        static string Usage
        {
            get { return @"Usage:\r\ntrain path.txt\r\nclassify url.\r\ntest"; }
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
            //IWordProcessor wp = lCaseStem(stopWords);

            _indexer = new WordIndexGenerator(new NoneWordTokenizer(excludeNumber: true), wp);
            _extractor = new AsyncWebText();

            var storage = new ClassifierDataInFile<string, string>(false);
            storage.Load();
            _storage = storage;
            _storableData = storage;

            _classifier = new FisherClassifier<string, string, string>(storage, ExtractFeatures, GetThreshold);
            //_classifier = new BayesianClassifier<string, string, string>(storage, ExtractFeatures, GetThreshold2);
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
                    return .001;
                case "vegetarian":
                    return .0001;
                case "vegan":
                    return .05;
                case "unknown":
                default:
                    return 0;
            }
        }

        static double GetThreshold2(string cat)
        {
            switch (cat.ToLower())
            {
                case "seafood":
                    return 1000;
                case "meat":
                    return 10;
                case "vegetarian":
                    return 1;
                case "vegan":
                    return 500;
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
            //FlushCache();
        }

        void FlushCache()
        {
            var d = new DirectoryInfo(Path.Combine(AssemblyDirectory, CACHED));
            if (d.Exists)
            {
                foreach (FileInfo f in d.EnumerateFiles("*.html"))
                {
                    f.Delete();
                }
            }
        }

        FileInfo GetCacheFile(Uri u)
        {
            if (u == null)
                throw new ArgumentNullException("u");

            var d = new DirectoryInfo(Path.Combine(AssemblyDirectory, CACHED));
            if (!d.Exists)
                d.Create();

            string fn = RE_FILE_SAFE.Replace(u.PathAndQuery, new MatchEvaluator(m =>
            {
                if (m.Index == 0)
                    return string.Empty;

                return "-";
            }));
            fn = Path.Combine(d.FullName, fn + ".html");
            return new FileInfo(fn);
        }

        const string RECIPES = "RecipesURL.txt";
        const string CACHED = "cached";

        public void Train()
        {
            Train(Path.Combine(AssemblyDirectory, RECIPES));
        }

        static readonly Regex RE_FILE_SAFE = new Regex(@"[^0-9a-z_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public void Train(string path)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("Train: {0}", path);
            var f = new FileInfo(path);
            if (!f.Exists)
                throw new FileNotFoundException(path);

            RemovePreviousTraining();

            int total = 0;
            int success = 0;
            using (FileStream fs = f.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(null);
                    string cat = arr.FirstOrDefault();
                    var url = new Uri(arr.LastOrDefault());

                    Interlocked.Increment(ref total);
                    FileInfo c = GetCacheFile(url);
                    string content;
                    if (c.Exists)
                        content = File.ReadAllText(c.FullName);
                    else
                        content = _extractor.Get(url).GetAwaiter().GetResult();
#if DEBUG
                    Console.WriteLine("{0}  {1}", cat, url.PathAndQuery);
#endif
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        _classifier.ItemTrain(content, cat);
                        Interlocked.Increment(ref success);
                    }
                    if (!c.Exists)
                        File.WriteAllText(c.FullName, content ?? string.Empty);
                }
            }
            Console.WriteLine("Success: {0:N0} of {1:N0} = {2:N0}%", success, total, success * 100f / total);
        }

		public void Test()
        {
            Test(Path.Combine(AssemblyDirectory, RECIPES));
        }

        public void Test(string path)
        {
            Train(path);

            Console.WriteLine("==============================================");
            Console.WriteLine("Test: {0}", path);

            int total = 0;
            int bad = 0;
            int failed = 0;
            var wrongs = new ConcurrentDictionary<string, int>();

            var f = new FileInfo(path);
            using (FileStream fs = f.OpenRead())
            using (var sr = new StreamReader(fs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(null);
                    string cat = arr.FirstOrDefault();
                    var url = new Uri(arr.LastOrDefault());

                    Interlocked.Increment(ref total);
                    FileInfo c = GetCacheFile(url);
                    string content;
                    if (c.Exists)
                        content = File.ReadAllText(c.FullName);
                    else
                        content = _extractor.Get(url).GetAwaiter().GetResult();

                    if (string.IsNullOrWhiteSpace(content))
                        Interlocked.Increment(ref failed);
                    else
                    {
                        Classification<string> r = _classifier.ItemClassify(content, "unknown");
                        if (string.Compare(r.Category, cat, true) != 0)
                        {
                            Interlocked.Increment(ref bad);
                            wrongs.AddOrUpdate(cat, 1, (k, v) => v + 1);
#if DEBUG
                            Console.WriteLine("Expects {0} but {1} @ {2:N0}% for {3}", cat, r.Category, r.Probability * 100, url.PathAndQuery);
#endif
                        }
                        //else
                        //    Console.Write(".");
                    }
                }
                Console.WriteLine("================================================");
                Console.WriteLine("{0:N0} wrong of {1:N0} ({2:N0}%)", bad, total, (bad * 100f) / total);
                Console.WriteLine("{0:N0} fails of {1:N0} ({2:N0}%)", failed, total, (failed * 100f) / total);
                foreach (var p in wrongs.OrderByDescending(o => o.Value))
                {
                    Console.WriteLine("{0} : {1:N0} ({2:N0}%)", p.Key, p.Value, (p.Value * 100f) / total);
                }
            }
        }

        public void Classify(params string[] urls)
        {
            foreach (string url in urls)
            {
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                var u = new Uri(url);
                string content = _extractor.Get(u).GetAwaiter().GetResult();
#if DEBUG
                //Console.WriteLine(content);
#endif
                Classification<string> r = _classifier.ItemClassify(content, "unknown");
                Console.WriteLine("{0} => {1} {2:N2}%", u.PathAndQuery, r.Category, r.Probability * 100);
            }
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
