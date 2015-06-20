using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ronin.ML.Classifier.Test
{
    public class TestClassifierDataInMongoDB<F, C> : ClassifierDataInMongoDB<F, C>, IDisposable
        where F : IEquatable<F>
        where C : IEquatable<C>
    {
        public TestClassifierDataInMongoDB()
            : base("mongodb://localhost/test_" + DateTime.UtcNow.Ticks)
        {

        }

        ~TestClassifierDataInMongoDB() { Dispose(); }
        int _disposed = 0;
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                _client.DropDatabaseAsync(_db.DatabaseNamespace.DatabaseName).Start(TaskScheduler.Default);
            }
        }
    }
}
