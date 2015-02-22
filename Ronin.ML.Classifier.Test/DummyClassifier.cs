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
	class DummyClassifier : AbstractClassifier<string, string, Bucket>
	{
		public DummyClassifier(
			IClassifierData<string, Bucket> data,
			Func<string, IEnumerable<string>> getFeatures)
			: base(data, getFeatures)
		{

		}

		public override double ItemProbability(string document, Bucket cat)
		{
			throw new NotImplementedException();
		}
	}
}
