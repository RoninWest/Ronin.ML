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
			data.TrainingData.ForEach(t => logic.ItemTrain(t.Data, t.Bucket));

			double goodProb = logic.ItemProbability(data.TestData, Bucket.GOOD);
			double badProb = logic.ItemProbability(data.TestData, Bucket.BAD);
			if (data.Category == Bucket.GOOD)
			{
				Assert.Greater(goodProb, badProb);
				Assert.Greater(goodProb / badProb, data.TestResult);
			}
			else
			{
				Assert.Greater(badProb, goodProb);
				Assert.Greater(badProb / goodProb, data.TestResult);
			}
		}
	}

	
}
