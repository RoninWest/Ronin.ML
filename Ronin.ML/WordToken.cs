using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	/// <summary>
	/// Represent the result of a tokenized word
	/// </summary>
	public class WordToken
	{
		public WordToken(string word, int position)
		{
			Word = word;
			Position = position;
		}

		string _word;
		/// <summary>
		/// Extracted value of the word token
		/// </summary>
		public string Word
		{
			get { return _word; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentException("Word can not be null or blank");

				_word = value;
			}
		}

		int _pos;
		/// <summary>
		/// Relative word position to the start of the document
		/// </summary>
		public int Position
		{
			get { return _pos; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Position can not be < 0");

				_pos = value;
			}
		}
	}
}
