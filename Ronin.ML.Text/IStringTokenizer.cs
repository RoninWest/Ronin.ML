using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Chunk strings into words
	/// </summary>
	public interface IStringTokenizer
	{
		/// <summary>
		/// Tokenize string
		/// </summary>
		IEnumerable<WordToken> Process(string data);
	}
}
