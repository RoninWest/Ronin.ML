﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ronin.ML.Text;
using System.IO;
using System.Reflection;

namespace Ronin.ML.Util
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime started = DateTime.UtcNow;
			//NormalizeArgs(args);
			BuildWordIndex(args);

			Console.WriteLine("Took: {0}", DateTime.UtcNow - started);
#if DEBUG
			Console.WriteLine("\r\nPress ANY Key");
			Console.ReadKey();
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

			var ignoreFile = new FileInfo(Path.Combine(AssemblyDirectory, "WebWordIgnores.txt"));
			IWordProcessor ignores = new IgnoreWordFilter(stopWords, new WhiteSpaceTokenizer(), ignoreFile);
			IWordProcessor wp = lCaseStem(ignores);

			var indexer = new WordIndexGenerator(new NoneWordTokenizer(excludeNumber:true), wp);

			var logic = new WebTextExtractor(args.First());
			string content = logic.Get();
			
			WordIndex wi = indexer.Process(content);
			Print(wi);
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
