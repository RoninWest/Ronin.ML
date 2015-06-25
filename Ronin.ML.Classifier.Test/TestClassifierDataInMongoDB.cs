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

        public override void IncrementCategory(C cat)
        {
            base.IncrementCategory(cat);
            Thread.Sleep(1); //give time for the FS to catch up, async call works too well
        }

        public override void IncrementFeature(F feat, C cat)
        {
            base.IncrementFeature(feat, cat);
            Thread.Sleep(1); //give time for the FS to catch up, async call works too well
        }

        public override void RemoveCategory(C cat)
        {
            base.RemoveCategory(cat);
            Thread.Sleep(5); //give time for the FS to catch up, async call works too well
        }
    }
}
