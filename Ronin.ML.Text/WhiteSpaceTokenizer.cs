using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Simple string split by white space
	/// </summary>
	public sealed class WhiteSpaceTokenizer : RegularExpressionTokenizer
	{
		public WhiteSpaceTokenizer(bool invariant = false, int minLenth = 2, int maxLength = 50) 
			: base(BuildExpression(invariant), exclusion:false, minLenth:minLenth, maxLength:maxLength)
        {
		}

		static Regex BuildExpression(bool invariant)
		{
			var op = RegexOptions.Compiled | RegexOptions.Multiline;
			if (invariant)
				op |= RegexOptions.CultureInvariant;

			return new Regex(@"\S+", op);
		}
		
	}
}
