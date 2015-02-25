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
	public class BayesianWordClassifier : BayesianClassifier<string, string, Bucket>
	{
		static readonly WordIndexGenerator _wi;

		static BayesianWordClassifier()
		{
			var tokenizer = new NoneWordTokenizer();
			var processor = new CaseNormalizer(new StopWordFilter(new StemNormalizer(new CaseNormalizer(null))));
			_wi = new WordIndexGenerator(tokenizer, processor);
		}

		public BayesianWordClassifier() : base(
			new ClassifierDataInRAM<string, Bucket>(), 
			s => _wi.Process(s).Keys.ToArray(),
			b => (int)b + 2)
		{

		}

		public override Classification<Bucket> ItemClassify(string item, Bucket defaultCategory = default(Bucket))
		{
			return base.ItemClassify(item, defaultCategory);
		}
	}
}
