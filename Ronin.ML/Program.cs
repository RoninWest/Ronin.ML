﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	class Program
	{
		static void Main(string[] args)
		{
			IWordNormalizer wp = new CaseNormalizer(new StemNormalizer(null));
			foreach (string s in args)
			{
				if (string.IsNullOrWhiteSpace(s))
					continue;

				var wc = new WordContext(s);
				wp.Process(wc);
				Console.WriteLine("{0} ~~> {1}", wc.Original, wc.Result);
			}
#if DEBUG
			Console.WriteLine("Press ANY Key");
			Console.ReadKey();
#endif
		}
	}
}
