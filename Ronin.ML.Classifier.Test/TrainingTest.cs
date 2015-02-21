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
	[TestFixture(typeof(ClassifierDataInRAM<string, TrainingBucket>), typeof(NoneWordTokenizer))]
    public class TrainingTest
    {
		readonly IClassifierData<string, TrainingBucket> _dataSrc;
		readonly IStringTokenizer _tokenizer;

		public TrainingTest(Type dataLogic, Type tokenizer)
		{
			//init data src
			Assert.IsNotNull(dataLogic);
			_dataSrc = Activator.CreateInstance(dataLogic) as IClassifierData<string, TrainingBucket>;
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

		/// <summary>
		/// Train and then test with feature counting
		/// </summary>
		/// <param name="data">training data</param>
		/// <param name="word">word to test for feature count</param>
		/// <param name="bucket">category to test the word feature against</param>
		/// <returns>number of times the word feature shows up in the given bucket</returns>
		[TestCaseSource(typeof(TrainingSet), "Data")]
		public void TrainAndFeatureCount(TrainingSet set)
		{
			Assert.IsNotNull(set);

			TrainingBucket[] cats = _dataSrc.CategoryKeys().ToArray();
			cats.ForEach(_dataSrc.RemoveCategory); //cleanup existing names

			var cf = new Classifier<string, string, TrainingBucket>(_dataSrc, StringSplit);
			set.Inputs.ForEach(t => cf.Train(t.Data, t.Bucket));

			double v = _dataSrc.CountFeature(set.Word, set.Bucket);
			Assert.AreEqual(set.Returns, v);
		}
    }

}
