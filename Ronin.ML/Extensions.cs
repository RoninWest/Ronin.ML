using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	public static class Extensions
	{
		/// <summary>
		/// A for each enumerator
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> actor)
		{
			if (actor == null)
				throw new ArgumentNullException("actor");

			if (items == null)
				return;

			foreach (T it in items)
			{
				actor(it);
			}
		}
	}
}
