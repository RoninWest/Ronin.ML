using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML
{
	public interface IWordProcessor
	{
		void Process(WordContext word);
	}
}
