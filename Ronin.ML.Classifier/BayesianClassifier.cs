using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Naive Bayesian Classifier
	/// </summary>
	/// <typeparam name="T">Item type</typeparam>
	/// <typeparam name="F">Feature type</typeparam>
	/// <typeparam name="C">Category type</typeparam>
	public class BayesianClassifier<T, F, C> : AbstractClassifier<T, F, C>
	{
		protected readonly Func<C, double> _getThreshold;

		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="data">Required: data service provider</param>
		/// <param name="getFeatures">Required: feature extraction method</param>
		/// <param name="getThreshold">Required: minimum score to consider each item as such when compare to the others</param>
		public BayesianClassifier(
			IClassifierData<F, C> data, 
			Func<T, IEnumerable<F>> getFeatures,
			Func<C, double> getThreshold) 
			: base(data, getFeatures)
		{
			if (getThreshold == null)
				throw new ArgumentNullException("getThreshold");

			_getThreshold = getThreshold;
		}

		/// <summary>
		/// Calc all feature probability of item
		/// </summary>
		double TotalFeatureProbability(T item, C cat) 
		{
			IEnumerable<F> features = _getFeatures(item);
			double p = 1;
			features.ForEach(f => p *= this.FeatureWeightedProbability(f, cat, this.FeatureProbability));
			return p;
		}
		
		/// <summary>
		/// Calculate the probability of the entire item to see if it belongs in the provided category
		/// </summary>
		/// <param name="item">The item to be calculated probability for</param>
		/// <param name="cat">The category or bucket to check the item against</param>
		/// <returns>probability score between 0 and 1.  0 being not certain and 1 being absolutely certain.</returns>
		public override double ItemProbability(T item, C cat)
		{
			double catProb = (double)_data.CountCategory(cat) / _data.TotalCategoryItems();
			double totalProb = this.TotalFeatureProbability(item, cat);
			return totalProb * catProb;
		}

		/// <summary>
		/// Attempts to categorize an item into a specific bucket
		/// </summary>
		/// <param name="item">The item to place into a bucket</param>
		/// <param name="defaultCategory">The default bucket if none can be identified</param>
		/// <returns>The result of the categorization attempt with bucket and weighted score</returns>
		public override Classification<C> ItemClassify(T item, C defaultCategory = default(C))
		{
			var probs = new Dictionary<C, double>();
			double max = 0;
			C best = defaultCategory; //find the category with the highest probability
			foreach (C cat in _data.CategoryKeys())
			{
				if(cat == null)
					continue;

				double p = this.ItemProbability(item, cat);
				probs.Add(cat, p);
				if (p > max)
				{
					max = p;
					best = cat;
				}
			}

			foreach (C cat in probs.Keys) //make sure probability exceeds threshold * next best
			{
				if (cat.Equals(best))
					continue;

				double bestThreshold = _getThreshold(best);
				if (probs[cat] * _getThreshold(best) > probs[best])
				{
					best = defaultCategory;
					break;
				}
			}

			double noneBestPct = (from c in probs.Keys
								  where !c.Equals(best)
								  select probs[c] / max).Sum();
			return new Classification<C>(1 - noneBestPct, best);
		}
	}
}
