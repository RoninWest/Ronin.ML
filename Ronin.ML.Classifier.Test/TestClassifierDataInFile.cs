using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Ronin.ML.Classifier.Test
{
    public class TestClassifierDataInFile<F, C> : ClassifierDataInFile<F, C>
    {
        public TestClassifierDataInFile()
            : base(DefaultParams, null)
        {

        }

        public override DataFileParam WriteDataFile
        {
            get { return base.ReadDataFile; }
        }

        protected override void DisposeLogic()
        {
            base.DisposeLogic();
            File.Delete(DefaultParams.Categories.FullName);
            File.Delete(DefaultParams.Features.FullName);
        }

        new static DataFileParam DefaultParams
        {
            get
            {
                Guid g = Guid.NewGuid();
                return new DataFileParam(
                    Path.Combine(AssemblyDirectory, "features." + g + ".txt"),
                    Path.Combine(AssemblyDirectory, "categories." + g + ".txt")
                );
            }
        }

    }
}
