using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Use to keep stats for Bayesian classifier
	/// </summary>
    public class WordStat
    {
		public WordStat(string word)
		{
			Word = word;
		}

		string _word;
		/// <summary>
		/// Actual word being tracked
		/// </summary>
		public string Word
		{
			get { return _word; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException("Word can not be null or empty");

				_word = value;
			}
		}

		long _yes = 0;
		/// <summary>
		/// Count of all true cases
		/// </summary>
		public long Yes
		{
			get { return Interlocked.Read(ref _yes); }
			set { Interlocked.Exchange(ref _yes, value); }
		}

		long _no = 0;
		/// <summary>
		/// Count of all false cases
		/// </summary>
		public long No
		{
			get { return Interlocked.Read(ref _no); }
			set { Interlocked.Exchange(ref _no, value); }
		}

		long _match = 0;
		/// <summary>
		/// Count of all total matches
		/// </summary>
		public long Matches
		{
			get { return Interlocked.Read(ref _match); }
			set { Interlocked.Exchange(ref _match, value); }
		}
    }
}
