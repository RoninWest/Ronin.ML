using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Use to separate stuff into buckets
	/// </summary>
	/// <typeparam name="T">Item type to train, and test</typeparam>
	/// <typeparam name="F">Item feature type to compare for classification purpose</typeparam>
	/// <typeparam name="C">Item category type for bucketing</typeparam>
	/// <remarks>Thread safe!</remarks>
	public abstract class AbstractClassifier<T, F, C> : IClassifier<T, C>, IDisposable
	{
		protected readonly IClassifierData<F, C> _data;
		protected readonly Func<T, IEnumerable<F>> _getFeatures;

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="data">Required: data service provider</param>
		/// <param name="getFeatures">Required: feature extraction method</param>
		public AbstractClassifier(IClassifierData<F, C> data, Func<T, IEnumerable<F>> getFeatures) 
		{
			if (data == null)
				throw new ArgumentNullException("data");
			if (getFeatures == null)
				throw new ArgumentNullException("getFeatures");

			_data = data;
			_getFeatures = getFeatures;
		}

        ~AbstractClassifier() { Dispose(); }
        int _disposed = 0;
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (_data != null && _data is IDisposable)
                    (_data as IDisposable).Dispose(); //optionally dispose data layer if it can be
            }
        }

		/// <summary>
		/// Teach the classifier with this item and classify it as such category
		/// </summary>
		/// <param name="item">Item to train</param>
		/// <param name="category">category to classify item as</param>
		public virtual void ItemTrain(T item, C category)
		{
			if (item == null || item.Equals(default(T)))
				throw new ArgumentException("item can not be null or default");
			if (category == null)
				throw new ArgumentNullException("category can not be null");

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
		protected internal virtual double FeatureProbability(F feature, C category)
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
		protected internal virtual double FeatureWeightedProbability(F feature, C category, 
			Func<F, C, double> prf, 
			double weight = 1, double assumedProb = .5)
		{
			if (feature == null || feature.Equals(default(F)))
				throw new ArgumentException("feature can not be null or default");
			if (category == null)
				throw new ArgumentNullException("category can not be null");
			if (prf == null)
				throw new ArgumentNullException("prf");
			if (assumedProb > 1 || assumedProb < 0)
				throw new ArgumentOutOfRangeException("assumedProb > 1 || assumedProb < 0");

			double basicProb = prf(feature, category);
			long totals = (from c in _data.CategoryKeys()
						   select _data.CountFeature(feature, c)).Sum();

			//weighted average
			return ((weight * assumedProb) + (totals * basicProb)) / (weight + totals);
		}

		/// <summary>
		/// Calculate the probability of the entire item to see if it belongs in the provided category
		/// </summary>
		/// <param name="item">The item to be calculated probability for</param>
		/// <param name="cat">The category or bucket to check the item against</param>
		/// <returns>probability score between 0 and 1.  0 being not certain and 1 being absolutely certain.</returns>
		public abstract double ItemProbability(T item, C cat);

		/// <summary>
		/// Attempts to categorize an item into a specific bucket
		/// </summary>
		/// <param name="item">The item to place into a bucket</param>
		/// <param name="defaultCategory">The default bucket if none can be identified</param>
		/// <returns>The result of the categorization attempt with bucket and weighted score</returns>
		public abstract Classification<C> ItemClassify(T item, C defaultCategory = default(C));
	}

}
