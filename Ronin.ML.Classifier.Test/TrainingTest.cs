using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using Ronin.ML.Text;

namespace Ronin.ML.Classifier.Test
{
	/// <summary>
	/// Training tests
	/// </summary>
    [TestFixture(typeof(TestClassifierDataInMongoDB<string, TestBucket>), typeof(WhiteSpaceTokenizer))]
    [TestFixture(typeof(TestClassifierDataInMongoDB<string, TestBucket>), typeof(NoneWordTokenizer))]
    [TestFixture(typeof(TestClassifierDataInFile<string, TestBucket>), typeof(WhiteSpaceTokenizer))]
    [TestFixture(typeof(TestClassifierDataInFile<string, TestBucket>), typeof(NoneWordTokenizer))]
    [TestFixture(typeof(ClassifierDataInRAM<string, TestBucket>), typeof(WhiteSpaceTokenizer))]
    [TestFixture(typeof(ClassifierDataInRAM<string, TestBucket>), typeof(NoneWordTokenizer))]
    public class TrainingTest : IDisposable
    {
		readonly IClassifierData<string, TestBucket> _dataSrc;
        readonly IDataStorable _ds;
		readonly IStringTokenizer _tokenizer;

		public TrainingTest(Type dataLogic, Type tokenizer)
		{
			//init data src
			Assert.IsNotNull(dataLogic);
			_dataSrc = Activator.CreateInstance(dataLogic) as IClassifierData<string, TestBucket>;
			Assert.IsNotNull(_dataSrc);

            if (_dataSrc is IDataStorable)
                _ds = _dataSrc as IDataStorable;

			//init tokenizer
			Assert.IsNotNull(tokenizer);
			_tokenizer = Activator.CreateInstance(tokenizer) as IStringTokenizer;
			Assert.IsNotNull(_tokenizer);
		}

        ~TrainingTest() { Dispose(); }
        int _disposed = 0;
        [TestFixtureTearDown]
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (_ds != null)
                    _ds.Dispose();
                else if (_dataSrc != null && _dataSrc is IDisposable)
                    (_dataSrc as IDisposable).Dispose();
            }
        }

		IEnumerable<string> StringSplit(string s)
		{
			IEnumerable<WordToken> tokens = _tokenizer.Process(s);
			return from t in tokens select t.Word;
		}

		DummyClassifier BuildAndTrain(TrainingSet set)
		{
			Assert.IsNotNull(set);

			TestBucket[] cats = _dataSrc.CategoryKeys().ToArray();
			cats.ForEach(_dataSrc.RemoveCategory); //cleanup existing names
			Assert.IsEmpty(_dataSrc.CategoryKeys());

            if (_ds != null)
            {
                _ds.Save();
                Assert.IsEmpty(_dataSrc.CategoryKeys());

                _ds.Load();
                Assert.IsEmpty(_dataSrc.CategoryKeys());
            }

			var cf = new DummyClassifier(_dataSrc, StringSplit);
			set.TrainingData.ForEach(t => cf.ItemTrain(t.Data, t.Bucket));
            
            if(_ds != null)
            {
                TestBucket[] catKeys = _dataSrc.CategoryKeys().ToArray();
                _ds.Save();
                CollectionAssert.AreEqual(catKeys, _dataSrc.CategoryKeys());

                _ds.Load();
                CollectionAssert.AreEqual(catKeys, _dataSrc.CategoryKeys());

                if (_dataSrc is ClassifierDataInFile<string, TestBucket>)
                {
                    var df = _dataSrc as ClassifierDataInFile<string, TestBucket>;
                    Assert.IsTrue(File.Exists(df.ReadDataFile.Categories.FullName));
                    Assert.IsTrue(File.Exists(df.ReadDataFile.Features.FullName));
                }
            }

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
			Assert.IsNotNull (cf);
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
