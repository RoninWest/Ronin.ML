using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ronin.ML.Classifier
{
	/// <summary>
	/// Generic in memory data provider for classifier logic,
	/// backed by physical file (load on ctor init and save on dispose/destructor)
	/// </summary>
	/// <typeparam name="F">any type for feature</typeparam>
	/// <typeparam name="C">any type for category</typeparam>
    public class ClassifierDataInFile<F, C> : ClassifierDataInRAM<F, C>, IDataStorable
	{
		public ClassifierDataInFile()
			: this(true)
		{
		}

		public ClassifierDataInFile(bool readOnly)
			: this(DefaultParams, write: (readOnly ? null : DefaultParams))
		{
		}

		protected static DataFileParam DefaultParams
		{
			get
			{
				return new DataFileParam(
					Path.Combine(AssemblyDirectory, "features.txt"),
					Path.Combine(AssemblyDirectory, "categories.txt")
				);
			}
		}

		protected static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

        readonly DataFileParam _read;
        public virtual DataFileParam ReadDataFile { get { return _read; } }

        readonly DataFileParam _write;
        public virtual DataFileParam WriteDataFile { get { return _write; } }

		public ClassifierDataInFile(DataFileParam read, DataFileParam write = null)
		{
            if (read == null)
                throw new ArgumentNullException("read");

            _read = read;
            _write = write;
            Load();
		}

        protected virtual T Deserialize<T>(FileInfo f)
            where T : class
        {
            T res = default(T);
            if (!File.Exists(f.FullName))
                return res;

            var bf = new BinaryFormatter();
            object o;
            using (FileStream fs = f.OpenRead())
            {
                o = bf.Deserialize(fs);
            }
            res = o as T;
            return res;
        }

        readonly object _saveLock = new object();

        public virtual void Load()
        {
            lock (_saveLock)
            {
                IDictionary<C, long> cats = Deserialize<IDictionary<C, long>>(ReadDataFile.Categories);
                IDictionary<F, FeatureCount<C>> feats = Deserialize<IDictionary<F, FeatureCount<C>>>(ReadDataFile.Features);

                cats.ForEach(p => _cc.AddOrUpdate(p.Key, p.Value, (k, v) => v));
                feats.ForEach(p => _fc.AddOrUpdate(p.Key, p.Value, (k, v) => v));
            }
        }

        protected virtual void Serialize<T>(T item, FileInfo f)
            where T : class
        {
            var bf = new BinaryFormatter();
            using (FileStream fs = f.OpenWrite())
            {
                bf.Serialize(fs, item);
            }
        }

        public virtual void Save()
        {
            if (WriteDataFile == null)
                throw new InvalidOperationException("This classifier is read only.  Please provide a valid write DataFileParam at instantiation.");

            lock (_saveLock)
            {
                Serialize(Categories, WriteDataFile.Categories);
                Serialize(Features, WriteDataFile.Features);
            }
        }

        ~ClassifierDataInFile() { Dispose(); }
        int _disposed = 0;
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                DisposeLogic();
            }
        }

        protected virtual void DisposeLogic()
        {
            if (_write != null)
                Save();
        }

	}
}
