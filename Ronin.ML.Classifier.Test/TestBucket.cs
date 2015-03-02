using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier.Test
{
	/// <summary>
	/// bucket results into these
	/// </summary>
	public enum TestBucket : int
	{
		UNKNOWN = 0,
		GOOD = 1,
		BAD = 2,
	}
}
