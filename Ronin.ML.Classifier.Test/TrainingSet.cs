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
		public TrainingSet(IEnumerable<Training<string>> inputs, string word, TrainingBucket bucket, double returns = 0)
		{
			Assert.IsNotEmpty(Inputs = inputs);
			Word = word;
			Bucket = bucket;
			Returns = returns;
		}

		public string Word { get; set; }

		public TrainingBucket Bucket { get; set; }

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
		/// Simple string training data for feature
		/// </summary>
		public static IEnumerable<TrainingSet> FeatureData
		{
			get
			{
				var inputs = new[]
				{ 
					new Training<string>(TrainingBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TrainingBucket.BAD, "make quick money in the online casino"),

					new Training<string>(TrainingBucket.GOOD, "no body owns the water"),
					new Training<string>(TrainingBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TrainingBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "quick", TrainingBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "quick", TrainingBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "fox", TrainingBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "fox", TrainingBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "dog", TrainingBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "dog", TrainingBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "rabbit", TrainingBucket.GOOD, 1d);
				yield return new TrainingSet(inputs, "rabbit", TrainingBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "jumps", TrainingBucket.GOOD, 2d);
				yield return new TrainingSet(inputs, "jumps", TrainingBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "money", TrainingBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "money", TrainingBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "casino", TrainingBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, "casino", TrainingBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "online", TrainingBucket.BAD, 2d);
				yield return new TrainingSet(inputs, "make", TrainingBucket.BAD, 1d);
				yield return new TrainingSet(inputs, "buy", TrainingBucket.BAD, 1d);

				yield return new TrainingSet(inputs, "chicken", TrainingBucket.BAD, 0d);
				yield return new TrainingSet(inputs, "chicken", TrainingBucket.GOOD, 0d);
				yield return new TrainingSet(inputs, string.Empty, TrainingBucket.BAD, 0d);
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
					new Training<string>(TrainingBucket.GOOD, "the quick brown fox jumps over the lazy dog"),
					new Training<string>(TrainingBucket.BAD, "make quick money in the online casino"),
					new Training<string>(TrainingBucket.GOOD, "the fox in the sox"),
					new Training<string>(TrainingBucket.GOOD, "no body owns the water"),
					new Training<string>(TrainingBucket.GOOD, "the white rabbit jumps fences"),
					new Training<string>(TrainingBucket.BAD, "buy pharmaceuticals online now"),
				};
				yield return new TrainingSet(inputs, "pharmaceuticals", TrainingBucket.GOOD, 0);
				yield return new TrainingSet(inputs, "pharmaceuticals", TrainingBucket.BAD, .5);
				yield return new TrainingSet(inputs, "quick", TrainingBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "quick", TrainingBucket.BAD, .5);
				yield return new TrainingSet(inputs, "fox", TrainingBucket.GOOD, .5);
				yield return new TrainingSet(inputs, "fox", TrainingBucket.BAD, 0);
				yield return new TrainingSet(inputs, "dog", TrainingBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "dog", TrainingBucket.BAD, 0);
				yield return new TrainingSet(inputs, "rabbit", TrainingBucket.GOOD, .25);
				yield return new TrainingSet(inputs, "rabbit", TrainingBucket.BAD, 0);
				yield return new TrainingSet(inputs, "online", TrainingBucket.GOOD, 0);
				yield return new TrainingSet(inputs, "online", TrainingBucket.BAD, 1);
			}
		}
	}
}
