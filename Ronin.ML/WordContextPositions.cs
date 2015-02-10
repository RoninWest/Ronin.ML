using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	/// <summary>
	/// Inverted word index
	/// </summary>
	public class WordContextPositions
	{
		readonly WordContext _word;
		public WordContextPositions(WordContext word, IEnumerable<int> positions = null)
		{
			if (word == null)
				throw new ArgumentNullException("word");

			_word = word;
			positions.ForEach(n => _phash.Add(n));
        }

		/// <summary>
		/// Describes the word
		/// </summary>
		public WordContext Word { get { return _word; } }

		readonly HashSet<int> _phash = new HashSet<int>();
		/// <summary>
		/// Where the word is showing up
		/// </summary>
		public ICollection<int> Positions
		{
			get { return _phash; }
		}
	}
}
