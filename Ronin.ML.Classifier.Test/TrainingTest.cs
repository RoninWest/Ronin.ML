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
	/// Training tests
	/// </summary>
	[TestFixture(typeof(ClassifierDataInRAM<string, Bucket>), typeof(WhiteSpaceTokenizer))]
	[TestFixture(typeof(ClassifierDataInRAM<string, Bucket>), typeof(NoneWordTokenizer))]
    public class TrainingTest
    {
		readonly IClassifierData<string, Bucket> _dataSrc;
		readonly IStringTokenizer _tokenizer;

		public TrainingTest(Type dataLogic, Type tokenizer)
		{
			//init data src
			Assert.IsNotNull(dataLogic);
			_dataSrc = Activator.CreateInstance(dataLogic) as IClassifierData<string, Bucket>;
			Assert.IsNotNull(_dataSrc);

			//init tokenizer
			Assert.IsNotNull(tokenizer);
			_tokenizer = Activator.CreateInstance(tokenizer) as IStringTokenizer;
			Assert.IsNotNull(_tokenizer);
		}

		IEnumerable<string> StringSplit(string s)
		{
			IEnumerable<WordToken> tokens = _tokenizer.Process(s);
			return from t in tokens select t.Word;
		}

		DummyClassifier BuildAndTrain(TrainingSet set)
		{
			Assert.IsNotNull(set);

			Bucket[] cats = _dataSrc.CategoryKeys().ToArray();
			cats.ForEach(_dataSrc.RemoveCategory); //cleanup existing names
			Assert.IsEmpty(_dataSrc.CategoryKeys());

			var cf = new DummyClassifier(_dataSrc, StringSplit);
			set.TrainingData.ForEach(t => cf.ItemTrain(t.Data, t.Bucket));
			return cf;
		}

		/// <summary>
		/// ItemTrain and then test with feature counting
		/// </summary>
		/// <param name="set">training data</param>
		[TestCaseSource(typeof(TrainingSet), "FeatureData")]
		public void FeatureCountTest(TrainingSet set)
		{
			DummyClassifier cf = BuildAndTrain(set);
			double v = _dataSrc.CountFeature(set.TestData, set.Category);
			Assert.AreEqual(set.TestResult.ToString("N3"), v.ToString("N3"));
		}

		/// <summary>
		/// ItemTrain and test with basic probability
		/// </summary>
		/// <param name="set">training data</param>
		[TestCaseSource(typeof(TrainingSet), "BasicProbabilityData")]
		public void BasicProbabilityTest(TrainingSet set)
		{
			DummyClassifier cf = BuildAndTrain(set);
			double v = cf.FeatureProbability(set.TestData, set.Category);
			Assert.AreEqual(set.TestResult.ToString("N3"), v.ToString("N3"));
		}

		/// <summary>
		/// ItemTrain and test with weighted probability
		/// </summary>
		/// <param name="set">training data</param>
		[TestCaseSource(typeof(TrainingSet), "WeightedProbabilityData")]
		public void WeightedProbabilityTest(TrainingSet set)
		{
			DummyClassifier cf = BuildAndTrain(set);
			double v = cf.FeatureWeightedProbability(set.TestData, set.Category, cf.FeatureProbability);
			Assert.AreEqual(set.TestResult.ToString("N3"), v.ToString("N3"));
		}
    }

}
