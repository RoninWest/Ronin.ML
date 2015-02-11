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
			Console.WriteLine("\r\nPress ANY Key");
			Console.ReadKey();
#endif
		}

		static void BuildWordIndex(string[] args)
		{
			if (args.Length == 0 || !Uri.IsWellFormedUriString(args.FirstOrDefault(), UriKind.Absolute))
				throw new InvalidOperationException("please provide a valid absolute URL");

			var logic = new WebTextExtractor(args.First());
			string content = logic.Get();

			Func<IWordProcessor, IWordProcessor> lCaseStem = n => new LengthFilter(
				new CaseNormalizer(
					new StemNormalizer(n)
				), 
			min:3);

			IWordProcessor stopWords = new StopWordFilter(lCaseStem(null), TextLanguage.Default);
			IWordProcessor wp = lCaseStem(stopWords);

			var indexer = new WordIndexGenerator(new NoneWordTokenizer(excludeNumber:true), wp);
			WordIndex wi = indexer.Process(content);
			if (wi == null)
				Console.WriteLine("<null>");
			else if (wi.Count == 0)
				Console.WriteLine("<empty>");
			else
			{
				var reOrder = from p in wi
							  orderby p.Value.Count descending, p.Key.Length descending, p.Value.First() ascending
							  select p;
				foreach (var p in reOrder.Take(100))
				{
					Console.Write("{0}:{1}, ", p.Key, p.Value.Count);
				}
				Console.WriteLine("\r\n====================\r\nTotal Words: {0:N0}", wi.Count);
			}
		}

		static void NormalizeArgs(string[] args)
		{
			Func<IWordProcessor, IWordProcessor> lCaseStem = n => new LengthFilter(
				new CaseNormalizer(
					new StemNormalizer(n)
				)
			);

			IWordProcessor stopWords = new StopWordFilter(lCaseStem(null), TextLanguage.Default);
			IWordProcessor wp = lCaseStem(stopWords);
			foreach (string s in args)
			{
				if (string.IsNullOrWhiteSpace(s))
					continue;

				var wc = new WordContext(s);
				wp.Process(wc);
				Console.WriteLine("{0,-20} ==> {1}", wc.Original, wc.Result);
			}
		}

	}
}
