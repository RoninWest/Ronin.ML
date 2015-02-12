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
	/// <remarks>Thread safe!</remarks>
	public class Classifier<K, V>
	{
		readonly Func<IDictionary<K, V>> _getFeatures;

		public Classifier(Func<IDictionary<K, V>> getFeatures) 
		{
			if (getFeatures == null)
				throw new ArgumentNullException("getFeatures");

			_getFeatures = getFeatures;
		}

		#region Features

		readonly ConcurrentDictionary<string, FeatureCount> _fc = new ConcurrentDictionary<string, FeatureCount>();
		/// <summary>
		/// Store feature stats
		/// </summary>
		public IDictionary<string, FeatureCount> Features
		{
			get { return _fc; }
		}

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature name</param>
		/// <param name="cat">category name</param>
		/// <returns>new category count value</returns>
		public long IncrementFeature(string feat, string cat)
		{
			FeatureCount fc = _fc.AddOrUpdate(feat,
				f => new FeatureCount { { cat, 1 } },
				(f, cv) =>
					{
						cv.Increment(cat);
						return cv;
					});
			return fc[cat];
		}

		/// <summary>
		/// Return the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature name</param>
		/// <param name="cat">category name</param>
		/// <returns>count value</returns>
		public long CountFeature(string feat, string cat)
		{
			FeatureCount fc;
			if (_fc.TryGetValue(feat, out fc) && fc.ContainsKey(cat))
				return fc[cat];

			return 0;
		}

		#endregion

		#region Categories

		readonly ConcurrentDictionary<string, long> _cc = new ConcurrentDictionary<string, long>();
		/// <summary>
		/// Store category stats
		/// </summary>
		public IDictionary<string, long> Categories
		{
			get { return _cc; }
		}

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category name</param>
		/// <returns>new value</returns>
		public long IncrementCategory(string cat)
		{
			return _cc.AddOrUpdate(cat, 1, (c, v) => v + 1);
		}

		/// <summary>
		/// Count category
		/// </summary>
		/// <param name="cat">category name</param>
		/// <returns>current value</returns>
		public long CountCategory(string cat)
		{
			long v = 0;
			_cc.TryGetValue(cat, out v);
			return v;
		}

		#endregion
	}
}
