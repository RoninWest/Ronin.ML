using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// A variation of Bayesian Classifier that utilize a better way to compute probability 
	/// by using the "Fisher Kernel Method" <see cref="http://stats.stackexchange.com/questions/73208/bayesian-fisher-method-model-very-simple-data-to-get-discriminants"/>
	/// </summary>
	/// <typeparam name="T">Item type</typeparam>
	/// <typeparam name="F">Feature type</typeparam>
	/// <typeparam name="C">Category type</typeparam>
	public class FisherClassifier<T, F, C> : BayesianClassifier<T, F, C>
	{
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="data">Required: data service provider</param>
		/// <param name="getFeatures">Required: feature extraction method</param>
		/// <param name="getThreshold">Required: minimum score to consider each item as such when compare to the others</param>
		public FisherClassifier(
			IClassifierData<F, C> data, 
			Func<T, IEnumerable<F>> getFeatures,
			Func<C, double> getThreshold) 
			: base(data, getFeatures, getThreshold)
		{
			
		}

		/// <summary>
		/// Fisher method's override
		/// </summary>
		protected internal override double FeatureProbability(F feature, C category)
		{
			double clf = base.FeatureProbability(feature, category); //frequency of this feature in this category
			if (clf == 0)
				return 0;

			double freqsum = (from c in _data.CategoryKeys()
							  let p = base.FeatureProbability(feature, c)
							  select p).Sum(); //frequency of this feature in all categories

			//probability is the frequency in this category divided by the overall frequency
			double probability = freqsum == 0 ? 0 : clf / freqsum;
			return probability;
		}

		//Fisher method's override
		public override double ItemProbability(T item, C cat)
		{
			double p = 1; //multiply all probabilities together
			IEnumerable<F> features = _getFeatures(item);
			int fcount = 0;
			foreach(F f in features)
			{
				fcount++;
				p *= FeatureWeightedProbability(f, cat, FeatureProbability);
			}

			double fscore = -2 * Math.Log(p);
			double prob = InverseChiSquare(fscore, fcount * 2);
			return prob;
		}

		static double InverseChiSquare(double chi, double df)
		{
			double m = chi / 2;
			double term;
			double sum = term = Math.Exp(-m);

			int rangeMax = (int)Math.Floor(df / 2);
			for (int i = 1; i <= rangeMax; i++)
			{
				term *= m / i;
				sum += term;
			}
			return Math.Min(sum, 1);
		}

		//Fisher's override
		public override Classification<C> ItemClassify(T item, C defaultCategory = default(C))
		{
			C best = defaultCategory;
			double max = 0;
			foreach (C c in _data.CategoryKeys())
			{
				double p = this.ItemProbability(item, c);
				if (p > _getThreshold(c) && p > max)
				{
					best = c;
					max = p;
				}
			}
			return new Classification<C>(max, best);
		}
	}
}
