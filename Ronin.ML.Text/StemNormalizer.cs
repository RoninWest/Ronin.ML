using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iveonik.Stemmers;

namespace Ronin.ML.Text
{
	/// <summary>
	/// SnowBall stemmer for common languages
	/// </summary>
	public class StemNormalizer : IWordNormalizer
	{
		readonly IStemmer _logic;
		readonly IWordNormalizer _processor;

		public StemNormalizer(IWordNormalizer processor, StemLanguage language = StemLanguage.Default)
		{
			_processor = processor;
			switch (language)
			{
				case StemLanguage.English:
					_logic = new EnglishStemmer();
					break;
				case StemLanguage.Spanish:
					_logic = new SpanishStemmer();
					break;
				case StemLanguage.French:
					_logic = new FrenchStemmer();
					break;
				case StemLanguage.German:
					_logic = new GermanStemmer();
					break;
				default:
					throw new ArgumentOutOfRangeException("Unknown Stemmer language: " + language);
			}
		}

		public void Process(WordContext word)
		{
			if (word.Result == null)
				return;

			word.Result = _logic.Stem(word.Result);

			if(_processor != null)
				_processor.Process(word);
		}
	}
}
