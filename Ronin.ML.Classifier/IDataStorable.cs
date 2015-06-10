using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier
{
    public interface IDataStorable : IDisposable
    {
        void Load();
        void Save();
    }
}
