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
	//[TestFixture(typeof(FisherWordClassifier))] //NOTE: this classifier is not working as expected. need to double check syntax for error
	[TestFixture(typeof(BayesianWordClassifier))]
	public class ClassifierTest<Cf>
		where Cf : IClassifier<string, TestBucket>, new()
	{
		/// <summary>
		/// Manually test document probability
		/// </summary>
		[TestCaseSource(typeof(TrainingSet), "BayesianClassifierData")]
		public void DocumentProbabilityTest(TrainingSet data) 
		{
			Cf logic = InitAndTrainClassifier(data);

			double goodProb = logic.ItemProbability(data.TestData, TestBucket.GOOD);
			double badProb = logic.ItemProbability(data.TestData, TestBucket.BAD);
			if (data.Category == TestBucket.GOOD)
			{
				Assert.Greater(goodProb, badProb);
				Assert.Greater(goodProb / badProb, data.TestResult);
			}
			else if (data.Category == TestBucket.BAD)
			{
				Assert.Greater(badProb, goodProb);
				Assert.Greater(badProb / goodProb, data.TestResult);
			}
			else
				Assert.AreEqual(TestBucket.UNKNOWN, data.Category);
		}

		/// <summary>
		/// Broiler training logic
		/// </summary>
		static Cf InitAndTrainClassifier(TrainingSet data)
		{
			Assert.IsNotNull(data);
			var logic = new Cf();

			Assert.IsNotEmpty(data.TrainingData);
			data.TrainingData.ForEach(t => logic.ItemTrain(t.Data, t.Bucket));
			return logic;
		}

		/// <summary>
		/// Test classification of items
		/// </summary>
		[TestCaseSource(typeof(TrainingSet), "BayesianClassifierData")]
		public void DocumentClassifyTest(TrainingSet data)
		{
			Cf logic = InitAndTrainClassifier(data);

			Classification<TestBucket> r = logic.ItemClassify(data.TestData, TestBucket.UNKNOWN); //reduce false positives
			Assert.IsNotNull(r);
			Assert.AreEqual(data.Category, r.Category);
			Assert.GreaterOrEqual(r.Probability, (double)1 - (1 / data.TestResult));
		}
	}

	
}
