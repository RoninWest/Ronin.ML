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

		/// <summary>
		/// Instantiate with expression and optional param
		/// </summary>
		/// <param name="expression">regular expression, required</param>
		/// <param name="exclusion">optional exclusion flag. when true will split by the provided expression and when false (default) will capture only the provided expression</param>
		public RegularExpressionTokenizer(Regex expression, bool exclusion = false)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

            _exp = expression;
			_exclusion = exclusion;
        }

		/// <summary>
		/// Required: regular expression
		/// </summary>
		public Regex Expression
		{
			get { return _exp; }
		}

		/// <summary>
		/// ReadOnly: optional exclusion flag. when true will split by the provided expression and when false (default) will capture only the provided expression.
		/// </summary>
		public bool Exclusion
		{
			get { return _exclusion; }
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
                for (int i = 0; i < mc.Count; i++)
				{
					Match cur = mc[i];
					Match last;
					int start, len;

					if (i == 0)
					{
						last = null;
						start = 0;
						len = cur.Index;
					}
					else
					{
						last = mc[i - 1];
						start = last.Index + last.Length;
						len = cur.Index - start;
						if (len <= 0)
							continue;
					}

					string value = data.Substring(start, len);
					if (!string.IsNullOrEmpty(value))
						wlist.Add(new WordToken(value, start));

					if(i == mc.Count - 1)
					{
						start = cur.Index + cur.Length;
						value = data.Substring(start);
						if(!string.IsNullOrEmpty(value))
							wlist.Add(new WordToken(value, start));
					}
				}
			}
			else
			{
				foreach (Match m in mc)
				{
					wlist.Add(new WordToken(m.Value, m.Index));
				}
			}
			return wlist;
		}
	}
}
