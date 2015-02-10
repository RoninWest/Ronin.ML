using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	/// <summary>
	/// Chunk strings into words
	/// </summary>
	public interface IStringTokenizer
	{
		IEnumerable<WordToken> Tokenize(string data);
	}
}
