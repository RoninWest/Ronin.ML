using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Simply lower case the word
	/// </summary>
	public class CaseNormalizer : IWordProcessor
	{
		readonly IWordProcessor _processor;

		public CaseNormalizer(IWordProcessor processor, CaseModification mod = CaseModification.Default, bool invariant = false)
		{
			_processor = processor;
			CaseModification = mod;
			Invariant = invariant;
        }

		public bool Invariant { get; set; }
		public CaseModification CaseModification { get; set; }

		public void Process(WordContext word)
		{
			if (word.Result == null)
				return;

			switch (CaseModification)
			{
				case CaseModification.Upper:
					word.Result = Invariant ? word.Result.ToUpperInvariant() : word.Result.ToUpper();
					break;
				case CaseModification.Lower:
				default:
					word.Result = Invariant ? word.Result.ToLowerInvariant() : word.Result.ToLower();
					break;
			}

			if (_processor != null)
				_processor.Process(word);
		}

	}
}
