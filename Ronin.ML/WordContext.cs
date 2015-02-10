using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	public class WordContext
	{
		public WordContext(string original)
		{
			if (string.IsNullOrWhiteSpace(original))
				throw new ArgumentException("Original can not be null or empty");

			_og = original;
			Result = original;
		}

		readonly string _og;
		/// <summary>
		/// Original word value pre-processing
		/// </summary>
		public string Original
		{
			get { return _og; }
		}

		public string Result { get; set; }
	}
}
