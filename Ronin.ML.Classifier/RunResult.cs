using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Ronin.ML.Classifier
{
	public class RunResult<C> : IComparable, IComparable<RunResult<C>>
	{
		public RunResult ()
		{
		}

		C _category;
		public C Category 
		{
			get { return _category; }
			set 
			{
				if (value == null || value.Equals (default(C)))
					throw new ArgumentException ("Category ");
			}
		}
		double _weight;
		public double Weight 
		{
			get { return _weight; }
			set 
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException ("Weight can not be < 0 or > 1");
				
				_weight = value;
			}
		}
			
		public int Total;
		public int Wrong;
		public int Fails;

		public int Success 
		{
			get { return Total - Wrong - Fails; }
		}

		/// <summary>
		/// Success percentage. Read only.
		/// </summary>
		public double SuccessPct 
		{
			get { return Total > 0 ? Math.Round(Success * 100d / Total, 3) : 0d; }
		}

		readonly ConcurrentDictionary<C, int> _wrongs = new ConcurrentDictionary<C, int>();
		public IDictionary<C, int> WrongDetails 
		{
			get { return _wrongs; }
		}

		public void CountFailed(C cat) 
		{
			Interlocked.Increment (ref Wrong);
			_wrongs.AddOrUpdate (cat, 1, (k, v) => v + 1);
		}

		public int CompareTo(RunResult<C> o) 
		{
			if (o == null)
				return 2;

			int r = SuccessPct.CompareTo (o.SuccessPct);
			if (r == 0) 
			{
				r = Fails.CompareTo (o.Fails) * -1;
				if (r == 0) 
					r = WrongStandardDeviation.CompareTo (o.WrongStandardDeviation) * -1;
			}
			return r;
		}

		public int CompareTo(object o) 
		{
			if (o == null)
				return 5;
			else if (!(o is RunResult<C>))
				return 3;
			else
				return CompareTo (o as RunResult<C>);
		}

		double WrongStandardDeviation 
		{
			get { return CalculateStdDev (from w in WrongDetails select (double)w.Value); }
		}

		double CalculateStdDev(IEnumerable<double> values)
		{   
			double ret = 0;
			if (values.Count() > 0) 
			{      
				//Compute the Average      
				double avg = values.Average();
				//Perform the Sum of (value-avg)_2_2      
				double sum = values.Sum(d => Math.Pow(d - avg, 2));
				//Put it all together      
				ret = Math.Sqrt((sum) / (values.Count()-1));   
			}   
			return ret;
		}

		/*
		readonly Dictionary<C, double> _threshold = new Dictionary<C, double>();
		public IDictionary<C, double> Threshold 
		{
			get { return _threshold; }
		}
		*/
	}
}

