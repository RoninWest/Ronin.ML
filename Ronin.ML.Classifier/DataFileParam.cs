using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Ronin.ML.Classifier
{
	public class DataFileParam
	{
		public DataFileParam(string features, string categories) 
			: this(new FileInfo(features), new FileInfo(categories))
		{

		}

		public DataFileParam(FileInfo features, FileInfo categories)
		{
			Features = features;
			Categories = categories;
		}

        FileInfo _features;
        public virtual FileInfo Features
        {
            get { return _features; }
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException("Features");

                _features = value;
            }
        }

        FileInfo _categories;
        public virtual FileInfo Categories
        {
            get { return _categories; }
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException("Categories");

                _categories = value;
            }
        }
	}
}
