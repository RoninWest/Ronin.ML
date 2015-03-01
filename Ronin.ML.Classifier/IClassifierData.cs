using System;
using System.Collections.Generic;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Data storage and provider for classifier logic
	/// </summary>
	/// <typeparam name="F">any type for feature</typeparam>
	/// <typeparam name="C">any type for category</typeparam>
	public interface IClassifierData<F, C>
	{
		/// <summary>
		/// List of all category keys
		/// </summary>
		IEnumerable<C> CategoryKeys();

		/// <summary>
		/// Count category
		/// </summary>
		/// <param name="cat">category value</param>
		/// <returns>current value</returns>
		long CountCategory(C cat);

		/// <summary>
		/// Return the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category value</param>
		/// <returns>count value</returns>
		long CountFeature(F feat, C cat);

		/// <summary>
		/// Increment Category Count
		/// </summary>
		/// <param name="cat">category value</param>
		void IncrementCategory(C cat);

		/// <summary>
		/// Increment the count for a feature/category pair
		/// </summary>
		/// <param name="feat">feature value</param>
		/// <param name="cat">category value</param>
		void IncrementFeature(F feat, C cat);

		/// <summary>
		/// Total number of items
		/// </summary>
		long TotalCategoryItems();

		/// <summary>
		/// Cleanup category data
		/// </summary>
		/// <param name="cat">value of category to clean up</param>
		void RemoveCategory(C cat);
	}
}
