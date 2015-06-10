using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Filters out string within a min or max limit
	/// </summary>
	public class LengthFilter : IWordProcessor
	{
		readonly IWordProcessor _processor;
		public LengthFilter(IWordProcessor processor, int min = MIN_DEFAULT, int max = MAX_DEFAULT)
		{
			_processor = processor;
			Min = min;
			Max = max;
		}

		const int MIN_DEFAULT = 2;
		int _min = MIN_DEFAULT;
		/// <summary>
		/// Minimum string length
		/// </summary>
		public int Min 
		{
			get { return _min; }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("Min < 1");

				_min = value;
			}
		}

		const int MAX_DEFAULT = 50;
		int _max = MAX_DEFAULT;
		/// <summary>
		/// Maximum string length
		/// </summary>
		public int Max
		{
			get { return _max; }
			set
			{
				if (value < Min)
					throw new ArgumentOutOfRangeException("Max < Min");

				_max = value;
			}
		}

		public void Process(WordContext word)
		{
			if (word.Result == null)
				return;

			if (word.Result.Length < Min || word.Result.Length > Max)
				word.Result = null;

			if (_processor != null)
				_processor.Process(word);
		}
	}
}
