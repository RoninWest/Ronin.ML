using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Use to separate stuff into buckets
	/// </summary>
	/// <typeparam name="T">Item type to train, and test</typeparam>
	/// <typeparam name="F">Item feature type to compare for classification purpose</typeparam>
	/// <remarks>Thread safe!</remarks>
	public class Classifier<T, F>
	{
		protected readonly IClassifierData<F> _data;
		readonly Func<T, IEnumerable<F>> _getFeatures;

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="data">Required: data service provider</param>
		/// <param name="getFeatures">Required: feature extraction method</param>
		public Classifier(IClassifierData<F> data, Func<T, IEnumerable<F>> getFeatures) 
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (getFeatures == null)
				throw new ArgumentNullException("getFeatures");

			_data = data;
			_getFeatures = getFeatures;
		}

		/// <summary>
		/// Train with this item and classify it as such category
		/// </summary>
		/// <param name="item">Item to train</param>
		/// <param name="category">category to classify item as</param>
		public virtual void Train(T item, string category)
		{
			if (item == null || item.Equals(default(T)))
				throw new ArgumentException("item can not be null or default");
			if (string.IsNullOrEmpty(category))
				throw new ArgumentException("category can not be null or empty");

			IEnumerable<F> features = _getFeatures(item);
			bool once = false;
			if (features != null)
			{
				foreach (F f in features)
				{
					_data.IncrementFeature(f, category);
					once = true;
				}
			}
			if(once)
				_data.IncrementCategory(category);
		}

		/// <summary>
		/// Calculate probability that a feature belongs to a category
		/// </summary>
		/// <param name="feature">Feature in question</param>
		/// <param name="category">Category to test</param>
		/// <returns>percentage value between 0 and 1. 1 being most likely and 0 being not</returns>
		public virtual double Probability(F feature, string category)
		{
			long cc = _data.CountCategory(category);
			if (cc == 0)
				return 0;

			long fc = _data.CountFeature(feature, category);
			return (double)fc / cc;
		}

		/// <summary>
		/// Better way to compute probability to not penalize things that doesn't appear as often
		/// </summary>
		/// <param name="feature">Feature in question</param>
		/// <param name="category">Category to test</param>
		/// <param name="prf">probability function</param>
		/// <param name="weight">weight of each feature</param>
		/// <param name="assumedProb">assumed probability for unknowns</param>
		/// <returns>percentage value between 0 and 1. 1 being most likely and 0 being not</returns>
		public virtual double WeightedProbability(F feature, string category, 
			Func<F, string, double> prf, 
			double weight = 1, double assumedProb = .5)
		{
			if (feature == null || feature.Equals(default(F)))
				throw new ArgumentException("feature can not be null or default");
			if (string.IsNullOrEmpty(category))
				throw new ArgumentException("category can not be null or empty");
			if (prf == null)
				throw new ArgumentNullException("prf");
			if (assumedProb > 1 || assumedProb < 0)
				throw new ArgumentOutOfRangeException("assumedProb > 1 || assumedProb < 0");

			double basicProb = prf(feature, category);
			long totals = (from c in _data.CategoryNames()
						   select _data.CountFeature(feature, c)).Sum();

			//weighted average
			return ((weight * assumedProb) + (totals * basicProb)) / (weight + totals);
		}
	}
}
