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
#if DEBUG
			Console.WriteLine("Press ANY Key");
			Console.ReadKey();
#endif
		}
	}
}
