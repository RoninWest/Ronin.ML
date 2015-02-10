﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iveonik.Stemmers;

namespace Ronin.ML
{
	/// <summary>
	/// SnowBall stemmer for common languages
	/// </summary>
	public class StemProcessor : IWordProcessor
	{
		readonly IStemmer _logic;
		readonly IWordProcessor _processor;

		public StemProcessor(IWordProcessor processor, StemLanguage language = StemLanguage.Default)
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
			word.Result = _logic.Stem(word.Result);

			if(_processor != null)
				_processor.Process(word);
		}
	}
}
