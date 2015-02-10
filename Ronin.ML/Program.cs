using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ronin.ML.Text;

namespace Ronin.ML
{
	class Program
	{
		static void Main(string[] args)
		{
			//NormalizeArgs(args);
			BuildWordIndex(args);
#if DEBUG
			Console.WriteLine("Press ANY Key");
			Console.ReadKey();
#endif
		}

		static void BuildWordIndex(string[] args)
		{
			if (args.Length == 0 || !Uri.IsWellFormedUriString(args.FirstOrDefault(), UriKind.Absolute))
				throw new InvalidOperationException("please provide a valid absolute URL");

			var logic = new WebTextExtractor(args.First());
			string content = logic.Get();

			Func<IWordNormalizer, IWordNormalizer> lCaseStem = n => new CaseNormalizer(new StemNormalizer(n));

			IWordNormalizer stopWords = new StopWordNormalizer(lCaseStem(null), TextLanguage.Default);
			IWordNormalizer wp = lCaseStem(stopWords);

			var indexer = new WordIndexGenerator(new WhiteSpaceTokenizer(), wp);
			WordIndex wi = indexer.Process(content);
			if (wi == null)
				Console.WriteLine("<null>");
			else if (wi.Count == 0)
				Console.WriteLine("<empty>");
			else
			{
				foreach (var p in wi)
				{
					Console.WriteLine("{0} @ [{1}]", p.Key, string.Join(",", p.Value));
				}
			}
		}

		static void NormalizeArgs(string[] args)
		{
			Func<IWordNormalizer, IWordNormalizer> lCaseStem = n => new CaseNormalizer(new StemNormalizer(n));

			IWordNormalizer stopWords = new StopWordNormalizer(lCaseStem(null), TextLanguage.Default);
			IWordNormalizer wp = lCaseStem(stopWords);
			foreach (string s in args)
			{
				if (string.IsNullOrWhiteSpace(s))
					continue;

				var wc = new WordContext(s);
				wp.Process(wc);
				Console.WriteLine("{0} ~~> {1}", wc.Original, wc.Result);
			}
		}

	}
}
