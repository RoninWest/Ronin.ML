using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Ronin.ML.Classifier.Test
{
	/// <summary>
	/// test data
	/// </summary>
	public class TrainingSet
	{
		public TrainingSet(IEnumerable<Training<string>> trainingData, string testData, TestBucket bucket, double returns = 0)
		{
			Assert.IsNotEmpty(TrainingData = trainingData);
			TestData = testData;
			Category = bucket;
			TestResult = returns;
		}

		/// <summary>
		/// Test input once training is completed
		/// </summary>
		public string TestData { get; set; }

		/// <summary>
		/// Category to bucket training data in and to use for testing of expected result
		/// </summary>
		public TestBucket Category { get; set; }

		/// <summary>
		/// Use this data for classifier training
		/// </summary>
		public IEnumerable<Training<string>> TrainingData
		{
			get;
			set;
		}

		/// <summary>
		/// Expected test result
		/// </summary>
		public double TestResult { get; set; }

		public override string ToString()
		{
			return string.Format("...,'{0}','{1}',{2}", TestData, Category, TestResult);
		}

		/// <summary>
		/// Simple string training data for feature
		/// </summary>
		public static IEnumerable<TrainingSet> FeatureData
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(TestBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TestBucket.BAD, "make quick money in the online casino"),

					new Training<string>(TestBucket.GOOD, "no body owns the water"),
					new Training<string>(TestBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TestBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "quick", TestBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "quick", TestBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "fox", TestBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "fox", TestBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "dog", TestBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "dog", TestBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "jumps", TestBucket.GOOD, 2d);
				yield return new TrainingSet(inputs, "jumps", TestBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "money", TestBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "money", TestBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "casino", TestBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "casino", TestBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "online", TestBucket.BAD, 2d);
				yield return new TrainingSet(inputs, "make", TestBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "buy", TestBucket.BAD, 1d);

				yield return new TrainingSet(inputs, "chicken", TestBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "chicken", TestBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, TestBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, TestBucket.BAD, 0d);
			}
		}

		/// <summary>
		/// Simple string training data for basic probability
		/// </summary>
		public static IEnumerable<TrainingSet> BasicProbabilityData
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(TestBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TestBucket.BAD, "make quick money in the online casino"),
					new Training<string>(TestBucket.GOOD, "the fox in the sox"),
					new Training<string>(TestBucket.GOOD, "no body owns the water"),
					new Training<string>(TestBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TestBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "pharmaceuticals", TestBucket.GOOD, 0);
				yield return new TrainingSet(inputs, "pharmaceuticals", TestBucket.BAD, .5);
				yield return new TrainingSet(inputs, "quick", TestBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "quick", TestBucket.BAD, .5);
				yield return new TrainingSet(inputs, "fox", TestBucket.GOOD, .5);
				yield return new TrainingSet(inputs, "fox", TestBucket.BAD, 0);
				yield return new TrainingSet(inputs, "dog", TestBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "dog", TestBucket.BAD, 0);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.BAD, 0);
				yield return new TrainingSet(inputs, "online", TestBucket.GOOD, 0);
				yield return new TrainingSet(inputs, "online", TestBucket.BAD, 1);
				yield return new TrainingSet(inputs, "chicken", TestBucket.GOOD, 0);
				yield return new TrainingSet(inputs, "chicken", TestBucket.BAD, 0);
			}
		}

		/// <summary>
		/// String training data for weighted probability
		/// </summary>
		public static IEnumerable<TrainingSet> WeightedProbabilityData
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(TestBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TestBucket.BAD, "make quick money in the online casino"),
					new Training<string>(TestBucket.GOOD, "the fox in the sox"),
					new Training<string>(TestBucket.GOOD, "no body owns the water"),
					new Training<string>(TestBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TestBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "pharmaceuticals", TestBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "pharmaceuticals", TestBucket.BAD, .5);
				yield return new TrainingSet(inputs, "quick", TestBucket.GOOD, .333);
				yield return new TrainingSet(inputs, "quick", TestBucket.BAD, .5);
				yield return new TrainingSet(inputs, "fox", TestBucket.GOOD, .5);
				yield return new TrainingSet(inputs, "fox", TestBucket.BAD, .167);
				yield return new TrainingSet(inputs, "dog", TestBucket.GOOD, .375);
				yield return new TrainingSet(inputs, "dog", TestBucket.BAD, .25);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.GOOD, .375);
				yield return new TrainingSet(inputs, "rabbit", TestBucket.BAD, .25);
				yield return new TrainingSet(inputs, "online", TestBucket.GOOD, .167);
				yield return new TrainingSet(inputs, "online", TestBucket.BAD, .833);
				yield return new TrainingSet(inputs, "chicken", TestBucket.GOOD, .5);
				yield return new TrainingSet(inputs, "chicken", TestBucket.BAD, .5);
			}
		}

		/// <summary>
		/// Text training data for Bayesian classifier
		/// </summary>
		public static IEnumerable<TrainingSet> BayesianClassifierData
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(TestBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TestBucket.BAD, "make quick money in the online casino"),
					new Training<string>(TestBucket.GOOD, "the fox in the sox"),
					new Training<string>(TestBucket.GOOD, "no body owns the water"),
					new Training<string>(TestBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TestBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "buying online pharmaceutical quickly", TestBucket.BAD, 10);
				yield return new TrainingSet(inputs, "buy online pharmaceuticals", TestBucket.BAD, 10);
				yield return new TrainingSet(inputs, "the rabbit jumps over the fox quick", TestBucket.GOOD, 20);
				yield return new TrainingSet(inputs, "jumping rabbits quickly over takes the brown fox", TestBucket.GOOD, 20);
				yield return new TrainingSet(inputs, "making quick money by going to online casinos", TestBucket.BAD, 10);
				yield return new TrainingSet(inputs, "brown fox buys online pharmaceutical then quickly makes money", TestBucket.BAD, 4);
			}
		}
	}
}
