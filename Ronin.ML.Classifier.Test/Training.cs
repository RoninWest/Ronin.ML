using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier.Test
{
	/// <summary>
	/// Training data
	/// </summary>
	/// <typeparam name="T">data type as input training</typeparam>
	public class Training<T>
	{
		public Training(Bucket bucket, T data = default(T))
		{
			Bucket = bucket;
			Data = data;
		}

		/// <summary>
		/// Data used for training
		/// </summary>
		public T Data { get; set; }

		/// <summary>
		/// Category to classify the training data as
		/// </summary>
		public Bucket Bucket { get; set; }
	}
}
