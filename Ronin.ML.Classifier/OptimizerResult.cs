using System;
using System.Collections.Generic;
using System.Linq;

namespace Ronin.ML.Classifier
{
	public class OptimizerResult<C>
	{
		public long TotalRuns;
		public long Errors;

		readonly Dictionary<C, double> _threshold = new Dictionary<C, double>();
		public IDictionary<C, double> Threshold 
		{
			get { return _threshold; }
		}
	}
}

