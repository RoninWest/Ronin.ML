using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NUnit.Framework;
using Ronin.ML.Text;

namespace Ronin.ML.Classifier.Test
{
	/// <summary>
	/// text only test classifier
	/// </summary>
	public class FisherWordClassifier : FisherClassifier<string, string, TestBucket>
	{
		static readonly WordIndexGenerator _wi;

		static FisherWordClassifier()
		{
			var tokenizer = new NoneWordTokenizer();
			var processor = new CaseNormalizer(new StopWordFilter(new StemNormalizer(new CaseNormalizer(null))));
			_wi = new WordIndexGenerator(tokenizer, processor);
		}

		public FisherWordClassifier()
			: base(
				new ClassifierDataInRAM<string, TestBucket>(),
				s => _wi.Process(s).Keys.ToArray(),
				b =>
				{
					switch (b)
					{
						case TestBucket.GOOD:
							return .5;
						case TestBucket.BAD:
							return 1;
						case TestBucket.UNKNOWN:
						default:
							return 0;
					}
				})
		{

		}
	}
}
