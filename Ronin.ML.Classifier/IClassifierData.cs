using System;
using System.Collections.Generic;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Data storage and provider for classifier logic
	/// </summary>
	public interface IClassifierData<F>
	{
		/// <summary>
		/// List of all category keys
		/// </summary>
		IEnumerable<string> CategoryNames();

		/// <summary>
		/// Count category
		/// </summary>
		/// <param name="cat">category name</param>
		/// <returns>current value</returns>
		long CountCategory(string cat);

		/// <summary>
		/// Return the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		/// <returns>count value</returns>
		long CountFeature(F feat, string cat);

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category name</param>
		void IncrementCategory(string cat);

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category name</param>
		void IncrementFeature(F feat, string cat);

		/// <summary>
		/// Total number of items
		/// </summary>
		long TotalCategoryItems();

		/// <summary>
		/// Cleanup category data
		/// </summary>
		/// <param name="cat">name of category to clean up</param>
		void RemoveCategory(string cat);
	}
}
