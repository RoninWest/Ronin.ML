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
	class DummyClassifier : AbstractClassifier<string, string, TestBucket>
	{
		public DummyClassifier(
			IClassifierData<string, TestBucket> data,
			Func<string, IEnumerable<string>> getFeatures)
			: base(data, getFeatures)
		{

		}

		public override double ItemProbability(string document, TestBucket cat)
		{
			throw new NotImplementedException();
		}

		public override Classification<TestBucket> ItemClassify(string item, TestBucket defaultCategory = default(TestBucket))
		{
			throw new NotImplementedException();
		}
	}
}
