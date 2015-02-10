using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Generate word index using tokenizer to chunk and normalizer to post process chunking results
	/// </summary>
	public sealed class WordIndexGenerator
	{
		readonly IStringTokenizer _tokenizer;
		readonly IWordNormalizer _normalizer;

		public WordIndexGenerator(IStringTokenizer tokenizer, IWordNormalizer normalizer)
		{
			if (tokenizer == null)
				throw new ArgumentNullException("tokenizer");
			if (normalizer == null)
				throw new ArgumentNullException("normalizer");

			_tokenizer = tokenizer;
			_normalizer = normalizer;
		}

		/// <summary>
		/// Generate word index using string builder.
		/// </summary>
		public WordIndex Process(string s)
		{
			if (string.IsNullOrEmpty(s))
				throw new ArgumentOutOfRangeException("s can not be null or empty");

			var wi = new WordIndex();
			foreach (WordToken wt in _tokenizer.Process(s))
			{
				if (string.IsNullOrEmpty(wt.Word))
					continue;

				var wc = new WordContext(wt.Word);
				_normalizer.Process(wc);
				wi.Add(wc.Result, wt.Position);
			}
			return wi;
		}
	}
}
