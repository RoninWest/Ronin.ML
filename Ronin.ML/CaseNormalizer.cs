using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	/// <summary>
	/// Simply lower case the word
	/// </summary>
	public class CaseNormalizer : IWordNormalizer
	{
		readonly bool _invariant;
		readonly CaseModification _mod;
		readonly IWordNormalizer _processor;

		public CaseNormalizer(IWordNormalizer processor, CaseModification mod = CaseModification.Default, bool invariant = false)
		{
			_processor = processor;
			_mod = mod;
			_invariant = invariant;
        }

		public void Process(WordContext word)
		{
			switch (_mod)
			{
				case CaseModification.Upper:
					word.Result = _invariant ? word.Result.ToUpperInvariant() : word.Result.ToUpper();
					break;
				case CaseModification.Lower:
				default:
					word.Result = _invariant ? word.Result.ToLowerInvariant() : word.Result.ToLower();
					break;
			}

			if (_processor != null)
				_processor.Process(word);
		}

	}
}
