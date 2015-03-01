using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Split string by none alpha numeric
	/// </summary>
	public sealed class NoneWordTokenizer : RegularExpressionTokenizer
	{
		public NoneWordTokenizer()
			: this(excludeNumber: false, invariant:false)
		{
		}

		public NoneWordTokenizer(bool excludeNumber = false, bool invariant = false)
			: base(BuildExpression(excludeNumber, invariant), exclusion:true)
		{
		}

		static Regex BuildExpression(bool excludeNumber, bool invariant)
		{
			var op = RegexOptions.Compiled | RegexOptions.Multiline;
			if (invariant)
				op |= RegexOptions.CultureInvariant;

			string re = @"\W\s`~!#^&*()\-_+=\[\]{}|\\:;""'?/";
			if (excludeNumber)
				re = @"\d" + re;

			return new Regex('[' + re + "]+", op);
		}
	}
}
