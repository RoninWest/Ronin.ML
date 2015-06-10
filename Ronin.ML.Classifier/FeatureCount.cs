using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Use to keep stats for a classifier feature.
	/// </summary>
	/// <remarks>Thread safe!</remarks>
    [Serializable]
    public class FeatureCount<C> : ConcurrentDictionary<C, long>, IDictionary<C, long>
    {
		/// <summary>
		/// Add 1 to key value counter
		/// </summary>
		/// <returns>return new value</returns>
		public long Increment(C key, int value = 1)
		{
			return this.AddOrUpdate(key, value, (k, v) => v + value);
		}

		/// <summary>
		/// Works in the same way as forced Add.  If key exists, force new value.
		/// </summary>
		public void Add(C key, long value)
		{
			this.AddOrUpdate(key, value, (k, v) => value);
		}
    }
}
