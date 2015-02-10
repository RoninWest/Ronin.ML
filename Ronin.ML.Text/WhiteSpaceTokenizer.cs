using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Chunk a string into words by white space
	/// </summary>
	public class WhiteSpaceTokenizer : IStringTokenizer
	{
		readonly Regex _spaceRE;

		public WhiteSpaceTokenizer(bool invariant = false)
		{
			var op = RegexOptions.Compiled | RegexOptions.Multiline;
			if (invariant)
				op |= RegexOptions.CultureInvariant;
            _spaceRE = new Regex(@"\s+", op);
		}

		public IEnumerable<WordToken> Process(string data)
		{
			ICollection<WordToken> wlist;
			MatchCollection mc = _spaceRE.Matches(data);
			if (mc.Count > 10000)
				wlist = new LinkedList<WordToken>();
			else
				wlist = new List<WordToken>(mc.Count);

			foreach (Match m in mc)
			{
				wlist.Add(new WordToken(m.Value, m.Index));
			}
			return wlist;
		}
	}
}
