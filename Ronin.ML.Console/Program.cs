﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ronin.ML.Text;
using Cnsl = System.Console;

namespace Ronin.ML.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime started = DateTime.UtcNow;
			//NormalizeArgs(args);
			BuildWordIndex(args);

			Cnsl.WriteLine("Took: {0}", DateTime.UtcNow - started);
#if DEBUG
			Cnsl.WriteLine("\r\nPress ANY Key");
			Cnsl.ReadKey();
#endif
		}

		static void BuildWordIndex(string[] args)
		{
			if (args.Length == 0 || !Uri.IsWellFormedUriString(args.FirstOrDefault(), UriKind.Absolute))
				throw new InvalidOperationException("please provide a valid absolute URL");

			Func<IWordProcessor, IWordProcessor> lCaseStem = n => new LengthFilter(
				new CaseNormalizer(
					new StemNormalizer(n)
				), 
			min:3);

			IWordProcessor stopWords = new StopWordFilter(lCaseStem(null), TextLanguage.Default);
			IWordProcessor wp = lCaseStem(stopWords);

			var indexer = new WordIndexGenerator(new NoneWordTokenizer(excludeNumber:true), wp);

			var logic = new WebTextExtractor(args.First());
			string content = logic.Get();
			
			WordIndex wi = indexer.Process(content);
			Print(wi);
		}

		static void Print(WordIndex wi)
		{
			if (wi == null)
				Cnsl.WriteLine("<null>");
			else if (wi.Count == 0)
				Cnsl.WriteLine("<empty>");
			else
			{
				var reOrder = from p in wi
							  orderby p.Value.Count descending, p.Key.Length descending, p.Value.First() ascending
							  select p;
				foreach (var p in reOrder.Take(100))
				{
					Cnsl.Write("{0}:{1}, ", p.Key, p.Value.Count);
				}
				Cnsl.WriteLine("\r\n====================\r\nTotal Words: {0:N0}", wi.Count);
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
				Cnsl.WriteLine("{0,-20} ==> {1}", wc.Original, wc.Result);
			}
		}

	}
}