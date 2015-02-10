using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Split string using provided expression
	/// </summary>
	public class RegularExpressionTokenizer : IStringTokenizer
	{
		readonly Regex _exp;
		readonly bool _exclusion;
		readonly int _minLen, _maxLen;

		/// <summary>
		/// Instantiate with expression and optional params
		/// </summary>
		/// <param name="expression">regular expression, required</param>
		/// <param name="exclusion">optional exclusion flag. when true will split by the provided expression and when false (default) will capture only the provided expression</param>
		/// <param name="minLenth">minimum word length. default is 2</param>
		/// <param name="maxLength">maximum word length. default is 50</param>
		public RegularExpressionTokenizer(Regex expression, bool exclusion = false, int minLenth = 2, int maxLength = 50)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (minLenth < 1)
				throw new ArgumentOutOfRangeException("minLength < 1");
			if (maxLength < minLenth)
				throw new ArgumentOutOfRangeException("maxLength < minLength");

            _exp = expression;
			_exclusion = exclusion;
			_minLen = minLenth;
			_maxLen = maxLength;
        }

		public virtual IEnumerable<WordToken> Process(string data)
		{
			ICollection<WordToken> wlist;
			MatchCollection mc = _exp.Matches(data);
			if (mc.Count > 10000)
				wlist = new LinkedList<WordToken>();
			else
				wlist = new List<WordToken>(mc.Count);

			if (_exclusion)
			{
                for (int i = 1; i < mc.Count; i++)
				{
					Match last = mc[i - 1];
					Match cur = mc[i];

					int start = last.Index + last.Length;
					int len = cur.Index - start;
					if (len <= 0)
						continue;

					string value = data.Substring(start, len);
					if (string.IsNullOrEmpty(value))
						continue;

					if (value.Length >= _minLen && value.Length <= _maxLen)
						wlist.Add(new WordToken(value, start));
				}
			}
			else
			{
				foreach (Match m in mc)
				{
					if(m.Length >= _minLen  && m.Length <= _maxLen)
						wlist.Add(new WordToken(m.Value, m.Index));
				}
			}
			return wlist;
		}
	}
}
