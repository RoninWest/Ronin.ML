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
		public TrainingSet(IEnumerable<Training<string>> trainingData, string testData, Bucket bucket, double returns = 0)
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
		public Bucket Category { get; set; }

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
					new Training<string>(Bucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(Bucket.BAD, "make quick money in the online casino"),

					new Training<string>(Bucket.GOOD, "no body owns the water"),
					new Training<string>(Bucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(Bucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "quick", Bucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "quick", Bucket.BAD, 1d);
				yield return new TrainingSet(inputs, "fox", Bucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "fox", Bucket.BAD, 0d);
				yield return new TrainingSet(inputs, "dog", Bucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "dog", Bucket.BAD, 0d);
				yield return new TrainingSet(inputs, "rabbit", Bucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "rabbit", Bucket.BAD, 0d);
				yield return new TrainingSet(inputs, "jumps", Bucket.GOOD, 2d);
				yield return new TrainingSet(inputs, "jumps", Bucket.BAD, 0d);
				yield return new TrainingSet(inputs, "money", Bucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "money", Bucket.BAD, 1d);
				yield return new TrainingSet(inputs, "casino", Bucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "casino", Bucket.BAD, 1d);
				yield return new TrainingSet(inputs, "online", Bucket.BAD, 2d);
				yield return new TrainingSet(inputs, "make", Bucket.BAD, 1d);
				yield return new TrainingSet(inputs, "buy", Bucket.BAD, 1d);

				yield return new TrainingSet(inputs, "chicken", Bucket.BAD, 0d);
				yield return new TrainingSet(inputs, "chicken", Bucket.GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, Bucket.GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, Bucket.BAD, 0d);
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
					new Training<string>(Bucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(Bucket.BAD, "make quick money in the online casino"),
					new Training<string>(Bucket.GOOD, "the fox in the sox"),
					new Training<string>(Bucket.GOOD, "no body owns the water"),
					new Training<string>(Bucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(Bucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "pharmaceuticals", Bucket.GOOD, 0);
				yield return new TrainingSet(inputs, "pharmaceuticals", Bucket.BAD, .5);
				yield return new TrainingSet(inputs, "quick", Bucket.GOOD, .25);
				yield return new TrainingSet(inputs, "quick", Bucket.BAD, .5);
				yield return new TrainingSet(inputs, "fox", Bucket.GOOD, .5);
				yield return new TrainingSet(inputs, "fox", Bucket.BAD, 0);
				yield return new TrainingSet(inputs, "dog", Bucket.GOOD, .25);
				yield return new TrainingSet(inputs, "dog", Bucket.BAD, 0);
				yield return new TrainingSet(inputs, "rabbit", Bucket.GOOD, .25);
				yield return new TrainingSet(inputs, "rabbit", Bucket.BAD, 0);
				yield return new TrainingSet(inputs, "online", Bucket.GOOD, 0);
				yield return new TrainingSet(inputs, "online", Bucket.BAD, 1);
				yield return new TrainingSet(inputs, "chicken", Bucket.GOOD, 0);
				yield return new TrainingSet(inputs, "chicken", Bucket.BAD, 0);
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
					new Training<string>(Bucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(Bucket.BAD, "make quick money in the online casino"),
					new Training<string>(Bucket.GOOD, "the fox in the sox"),
					new Training<string>(Bucket.GOOD, "no body owns the water"),
					new Training<string>(Bucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(Bucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "pharmaceuticals", Bucket.GOOD, .25);
				yield return new TrainingSet(inputs, "pharmaceuticals", Bucket.BAD, .5);
				yield return new TrainingSet(inputs, "quick", Bucket.GOOD, .333);
				yield return new TrainingSet(inputs, "quick", Bucket.BAD, .5);
				yield return new TrainingSet(inputs, "fox", Bucket.GOOD, .5);
				yield return new TrainingSet(inputs, "fox", Bucket.BAD, .167);
				yield return new TrainingSet(inputs, "dog", Bucket.GOOD, .375);
				yield return new TrainingSet(inputs, "dog", Bucket.BAD, .25);
				yield return new TrainingSet(inputs, "rabbit", Bucket.GOOD, .375);
				yield return new TrainingSet(inputs, "rabbit", Bucket.BAD, .25);
				yield return new TrainingSet(inputs, "online", Bucket.GOOD, .167);
				yield return new TrainingSet(inputs, "online", Bucket.BAD, .833);
				yield return new TrainingSet(inputs, "chicken", Bucket.GOOD, .5);
				yield return new TrainingSet(inputs, "chicken", Bucket.BAD, .5);
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
					new Training<string>(Bucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(Bucket.BAD, "make quick money in the online casino"),
					new Training<string>(Bucket.GOOD, "the fox in the sox"),
					new Training<string>(Bucket.GOOD, "no body owns the water"),
					new Training<string>(Bucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(Bucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "buy online pharmaceuticals", Bucket.GOOD, .007);
				yield return new TrainingSet(inputs, "buy online pharmaceuticals", Bucket.BAD, .069);
				yield return new TrainingSet(inputs, "the rabbit jumps over the fox quick", Bucket.GOOD, .021);
				yield return new TrainingSet(inputs, "the rabbit jumps over the fox quick", Bucket.BAD, .001);
			}
		}
	}
}
