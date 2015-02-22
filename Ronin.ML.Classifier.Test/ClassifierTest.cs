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
	/// Whole Classifier test
	/// </summary>
	[TestFixture(TypeArgs = new[] { typeof(BayesianWordClassifier) })]
	public class ClassifierTest<Cf>
		where Cf : IClassifier<string, Bucket>, new()
	{
		[TestCaseSource(typeof(TrainingSet), "BayesianClassifierData")]
		public void DocumentProbabilityTest(TrainingSet data) 
		{
			Assert.IsNotNull(data);

			var logic = new Cf();

			Assert.IsNotEmpty(data.TrainingData);
			data.TrainingData.ForEach(t => logic.ItemClassify(t.Data, t.Bucket));

			double prob = logic.ItemProbability(data.TestData, data.Category);
			Assert.AreEqual(data.TestResult.ToString("N3"), prob.ToString("N3"));
		}
	}

	
}
