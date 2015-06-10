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
	/// <typeparam name="F">any type for feature</typeparam>
	/// <typeparam name="C">any type for category</typeparam>
	public class ClassifierDataInRAM<F, C> : IClassifierData<F, C>
	{
		public ClassifierDataInRAM() { }

		public ClassifierDataInRAM(
			IDictionary<F, FeatureCount<C>> features, 
			IDictionary<C, long> categories)
		{
			features.ForEach(p => _fc.AddOrUpdate(p.Key, p.Value, (k, v) => p.Value));
			categories.ForEach(p => _cc.AddOrUpdate(p.Key, p.Value, (k, v) => p.Value));
		}

		readonly protected ConcurrentDictionary<F, FeatureCount<C>> _fc = new ConcurrentDictionary<F, FeatureCount<C>>();
		/// <summary>
		/// Store feature stats
		/// </summary>
		public virtual IDictionary<F, FeatureCount<C>> Features
		{
			get { return _fc; }
		}

		#region Features methods

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category value</param>
		public void IncrementFeature(F feat, C cat)
		{
			_fc.AddOrUpdate(feat,
				f => new FeatureCount<C> { { cat, 1 } },
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
		/// <param name="cat">category value</param>
		/// <returns>count value</returns>
		public long CountFeature(F feat, C cat)
		{
			FeatureCount<C> fc;
			if (_fc.TryGetValue(feat, out fc) && fc.ContainsKey(cat))
				return fc[cat];

			return 0;
		}

		#endregion

		readonly protected ConcurrentDictionary<C, long> _cc = new ConcurrentDictionary<C, long>();
		/// <summary>
		/// Store category stats
		/// </summary>
		public virtual IDictionary<C, long> Categories
		{
			get { return _cc; }
		}

		#region Categories methods

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category value</param>
		public virtual void IncrementCategory(C cat)
		{
			_cc.AddOrUpdate(cat, 1, (c, v) => v + 1);
		}

		/// <summary>
		/// Count category
		/// </summary>
		/// <param name="cat">category value</param>
		/// <returns>current value</returns>
		public virtual long CountCategory(C cat)
		{
			long v = 0;
			_cc.TryGetValue(cat, out v);
			return v;
		}

		/// <summary>
		/// Total number of items
		/// </summary>
		public virtual long TotalCategoryItems()
		{
			return _cc.Values.Sum();
		}

		/// <summary>
		/// List of all category keys
		/// </summary>
		public virtual IEnumerable<C> CategoryKeys()
		{
			return _cc.Keys;
		}

		/// <summary>
		/// Cleanup category data
		/// </summary>
		/// <param name="cat">value of category to clean up</param>
		public virtual void RemoveCategory(C cat)
		{
			long val;
			if (_cc.TryRemove(cat, out val))
			{
				F[] features = (from p in _fc
								where p.Value.ContainsKey(cat)
								select p.Key).ToArray();
				FeatureCount<C> fc;
				features.ForEach(f => _fc.TryRemove(f, out fc));
			}
		}

		#endregion
	}
}

