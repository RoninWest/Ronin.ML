using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Generic in memory data provider for classifier logic
	/// </summary>
	/// <typeparam name="F">any type</typeparam>
	public class ClassifierDataInRAM<F> : IClassifierData<F>
	{
		public ClassifierDataInRAM(
			IDictionary<F, FeatureCount> features = null, 
			IDictionary<string, long> categories = null)
		{
			features.ForEach(p => _fc.AddOrUpdate(p.Key, p.Value, (k, v) => p.Value));
			categories.ForEach(p => _cc.AddOrUpdate(p.Key, p.Value, (k, v) => p.Value));
		}

		readonly ConcurrentDictionary<F, FeatureCount> _fc = new ConcurrentDictionary<F, FeatureCount>();
		/// <summary>
		/// Store feature stats
		/// </summary>
		public IDictionary<F, FeatureCount> Features
		{
			get { return _fc; }
		}

		#region Features methods

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		public void IncrementFeature(F feat, string cat)
		{
			_fc.AddOrUpdate(feat,
				f => new FeatureCount { { cat, 1 } },
				(f, cv) =>
				{
					cv.Increment(cat);
					return cv;
				});
		}

		/// <summary>
		/// Return the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		/// <returns>count value</returns>
		public long CountFeature(F feat, string cat)
		{
			FeatureCount fc;
			if (_fc.TryGetValue(feat, out fc) && fc.ContainsKey(cat))
				return fc[cat];

			return 0;
		}

		#endregion

		readonly ConcurrentDictionary<string, long> _cc = new ConcurrentDictionary<string, long>();
		/// <summary>
		/// Store category stats
		/// </summary>
		public IDictionary<string, long> Categories
		{
			get { return _cc; }
		}

		#region Categories methods

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category name</param>
		public void IncrementCategory(string cat)
		{
			_cc.AddOrUpdate(cat, 1, (c, v) => v + 1);
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

		/// <summary>
		/// Total number of items
		/// </summary>
		public long TotalCategoryItems()
		{
			return _cc.Values.Sum();
		}

		/// <summary>
		/// List of all category keys
		/// </summary>
		public IEnumerable<string> CategoryNames()
		{
			return _cc.Keys;
		}

		#endregion
	}
}
