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
		readonly IClassifierData<F> _data;
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

		#region Features data methods

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		public void IncrementFeature(F feat, string cat)
		{
			_data.IncrementFeature(feat, cat);
		}

		/// <summary>
		/// Return the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		/// <returns>count value</returns>
		public long CountFeature(F feat, string cat)
		{
			return _data.CountFeature(feat, cat);
		}

		#endregion

		#region Categories data methods

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category name</param>
		/// <returns>new value</returns>
		public void IncrementCategory(string cat)
		{
			_data.IncrementCategory(cat);
		}

		/// <summary>
		/// Count category
		/// </summary>
		/// <param name="cat">category name</param>
		/// <returns>current value</returns>
		public long CountCategory(string cat)
		{
			return _data.CountCategory(cat);
		}

		/// <summary>
		/// Total number of items
		/// </summary>
		public long TotalCategoryItems
		{
			get { return _data.TotalCategoryItems(); }
		}

		/// <summary>
		/// List of all category keys
		/// </summary>
		public IEnumerable<string> CategoryNames
		{
			get { return _data.CategoryNames(); }
		}

		#endregion

		/// <summary>
		/// Train with this item and classify it as such category
		/// </summary>
		/// <param name="item">Item to train</param>
		/// <param name="category">category to classify item as</param>
		public void Train(T item, string category)
		{
			if (item == null || item.Equals(default(T)))
				throw new ArgumentException("item can not be null or default");

			IEnumerable<F> features = _getFeatures(item);
			bool once = false;
			if (features != null)
			{
				foreach (F f in features)
				{
					IncrementFeature(f, category);
					once = true;
				}
			}
			if(once)
				this.IncrementCategory(category);
		}
	}
}
