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
	[TestFixture(typeof(ClassifierDataInRAM<string>), typeof(NoneWordTokenizer))]
    public class TrainingTest
    {
		readonly IClassifierData<string> _dataSrc;
		readonly IStringTokenizer _tokenizer;

		public TrainingTest(Type dataLogic, Type tokenizer)
		{
			//init data src
			Assert.IsNotNull(dataLogic);
			_dataSrc = Activator.CreateInstance(dataLogic) as IClassifierData<string>;
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

			string[] cats = _dataSrc.CategoryNames().ToArray();
			cats.ForEach(_dataSrc.RemoveCategory); //cleanup existing names

			var cf = new Classifier<string, string>(_dataSrc, StringSplit);
			set.Inputs.ForEach(t => cf.Train(t.Data, t.Bucket));

			double v = _dataSrc.CountFeature(set.Word, set.Bucket);
			Assert.AreEqual(set.Returns, v);
		}
    }

	/// <summary>
	/// test data
	/// </summary>
	public class TrainingSet
	{
		public const string GOOD = "good", BAD = "bad";

		public TrainingSet(IEnumerable<Training<string>> inputs, string word, string bucket, double returns = 0)
		{
			Assert.IsNotEmpty(Inputs = inputs);
			Word = word;
			Bucket = bucket;
			Returns = returns;
		}

		public string Word { get; set; }

		public string Bucket { get; set; }

		public IEnumerable<Training<string>> Inputs
		{
			get;
			set;
		}

		public double Returns { get; set; }

		public override string ToString()
		{
			return string.Format("...,'{0}','{1}',{2}", Word, Bucket, Returns);
		}

		/// <summary>
		/// Simple string training data
		/// </summary>
		public static IEnumerable<TrainingSet> Data
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(BAD, "make quick money in the online casino"),

					new Training<string>(GOOD, "no body owns the water"),
					new Training<string>(GOOD, "the white rabbit jumps fences"),
					new Training<string>(BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "quick", GOOD, 1d);
				yield return new TrainingSet(inputs, "quick", BAD, 1d);
				yield return new TrainingSet(inputs, "fox", GOOD, 1d);
				yield return new TrainingSet(inputs, "fox", BAD, 0d);
				yield return new TrainingSet(inputs, "dog", GOOD, 1d);
				yield return new TrainingSet(inputs, "dog", BAD, 0d);
				yield return new TrainingSet(inputs, "rabbit", GOOD, 1d);
				yield return new TrainingSet(inputs, "rabbit", BAD, 0d);
				yield return new TrainingSet(inputs, "jumps", GOOD, 2d);
				yield return new TrainingSet(inputs, "jumps", BAD, 0d);
				yield return new TrainingSet(inputs, "money", GOOD, 0d);
				yield return new TrainingSet(inputs, "money", BAD, 1d);
				yield return new TrainingSet(inputs, "casino", GOOD, 0d);
				yield return new TrainingSet(inputs, "casino", BAD, 1d);
				yield return new TrainingSet(inputs, "online", BAD, 2d);
				yield return new TrainingSet(inputs, "make", BAD, 1d);
				yield return new TrainingSet(inputs, "buy", BAD, 1d);

				yield return new TrainingSet(inputs, "chicken", BAD, 0d);
				yield return new TrainingSet(inputs, "chicken", GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, BAD, 0d);
				yield return new TrainingSet(inputs, "casino", string.Empty, 0d);
			}
		}
	}
}
