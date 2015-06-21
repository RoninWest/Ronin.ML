using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier.Test
{
    public class TestClassifierDataInMongoDB<F, C> : ClassifierDataInMongoDB<F, C>
    {
        public TestClassifierDataInMongoDB()
            : base("mongodb://localhost/test_" + DateTime.UtcNow.Ticks)
        {

        }
    }
}
