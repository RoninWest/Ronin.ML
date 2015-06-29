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
	public class StemNormalizer : IWordProcessor
	{
		readonly IWordProcessor _processor;
		readonly IStemmer _logic;
		readonly TextLanguage _language;

		public StemNormalizer(IWordProcessor processor, TextLanguage language = TextLanguage.Default)
		{
			_processor = processor;

			switch (language)
			{
				case TextLanguage.English:
					_logic = new EnglishStemmer();
					break;
				case TextLanguage.Spanish:
					_logic = new SpanishStemmer();
					break;
				case TextLanguage.French:
					_logic = new FrenchStemmer();
					break;
				case TextLanguage.German:
					_logic = new GermanStemmer();
					break;
				default:
					throw new ArgumentOutOfRangeException("Unknown Stemmer language: " + language);
			}
			_language = language;
		}

		public TextLanguage Language
		{
			get { return _language; }
		}

		public void Process(WordContext word)
		{
			if (word.Result == null)
				return;

            try
            {
                word.Result = _logic.Stem(word.Result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
			if(_processor != null)
				_processor.Process(word);
		}
	}
}
