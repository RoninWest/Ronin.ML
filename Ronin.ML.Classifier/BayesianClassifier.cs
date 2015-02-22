using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Naive Bayesian Classifier
	/// </summary>
	public class BayesianClassifier<T, F, C> : AbstractClassifier<T, F, C>
	{
		public BayesianClassifier(IClassifierData<F, C> data, Func<T, IEnumerable<F>> getFeatures) 
			: base(data, getFeatures)
		{
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
	}
}
