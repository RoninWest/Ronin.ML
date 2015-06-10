using System;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// common classifier interface
	/// </summary>
	/// <typeparam name="T">type of item to be classified</typeparam>
	/// <typeparam name="C">category of item to be classified</typeparam>
	public interface IClassifier<T, C>
	{
		/// <summary>
		/// Calculate the probability of the entire item to see if it belongs in the provided category
		/// </summary>
		/// <param name="item">The item to be calculated probability for</param>
		/// <param name="cat">The category or bucket to check the item against</param>
		/// <returns>probability score between 0 and 1.  0 being not certain and 1 being absolutely certain.</returns>
		double ItemProbability(T item, C cat);

		/// <summary>
		/// Train with this item and classify it as such category
		/// </summary>
		/// <param name="item">Item to train</param>
		/// <param name="category">category to classify item as</param>
		void ItemTrain(T item, C category);

		/// <summary>
		/// Attempts to categorize an item into a specific bucket
		/// </summary>
		/// <param name="item">The item to place into a bucket</param>
		/// <param name="defaultCategory">The default bucket if none can be identified</param>
		/// <returns>The result of the categorization attempt with bucket and weighted score</returns>
		Classification<C> ItemClassify(T item, C defaultCategory = default(C));
	}
}
