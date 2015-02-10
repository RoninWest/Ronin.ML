﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	/// <summary>
	/// How to process the word to fix irregularity
	/// </summary>
	public interface IWordNormalizer
	{
		/// <summary>
		/// Normalize word
		/// </summary>
		/// <param name="word"></param>
		void Process(WordContext word);
	}
}
