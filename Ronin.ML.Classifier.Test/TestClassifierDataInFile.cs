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
            : base(DefaultParams, DefaultParams)
        {

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
                return new DataFileParam(
                    Path.Combine(AssemblyDirectory, "features." + Guid.NewGuid() + ".txt"),
                    Path.Combine(AssemblyDirectory, "categories." + Guid.NewGuid() + ".txt")
                );
            }
        }

    }
}
