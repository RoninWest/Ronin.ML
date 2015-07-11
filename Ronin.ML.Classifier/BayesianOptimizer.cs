using System;
using System.Collections.Generic;
using System.Linq;

namespace Ronin.ML.Classifier
{
	public class BayesianOptimizer<T, F, C>
	{
		readonly Func<BayesianClassifier<T, F, C>> _create;
		readonly Func<OptimizerResult<C>, double> _random;

		public BayesianOptimizer (
			Func<BayesianClassifier<T, F, C>> create,
			Func<OptimizerResult<C>, double> random)
		{
			if(create == null)
				throw new ArgumentNullException("create");
			if (random == null)
				throw new ArgumentNullException ("random");
			
			_create = create;
			_random = random;
		}
	}
}

