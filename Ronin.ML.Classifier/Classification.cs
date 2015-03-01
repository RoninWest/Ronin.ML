using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Bucketing result from the classifier from an input
	/// </summary>
	/// <typeparam name="C">bucket type</typeparam>
	public class Classification<C>
	{
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="cat">required bucket value</param>
		/// <param name="prob">required weight between 0 and 1</param>
		public Classification(double prob, C cat = default(C))
		{
			Category = cat;
			Probability = prob;
		}

		/// <summary>
		/// Bucket value
		/// </summary>
		public C Category { get; set; }

		double _prob;
		/// <summary>
		/// Weighted score between 0 and 1
		/// </summary>
		public double Probability
		{
			get { return _prob; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Probability < 0");
				if(value > 1)
					throw new ArgumentOutOfRangeException("Probability > 1");

				_prob = value;
			}
		}
	}
}
