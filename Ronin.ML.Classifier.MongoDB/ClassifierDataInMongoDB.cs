using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ronin.ML.Classifier
{
    /// <summary>
    /// Generic MongoDB data provider for classifier logic
    /// </summary>
    /// <typeparam name="F">any type for feature</typeparam>
    /// <typeparam name="C">any type for category</typeparam>
    public class ClassifierDataInMongoDB<F, C> : IClassifierData<F, C>
    {
        readonly IMongoClient _client;
        readonly IMongoDatabase _db;

        public ClassifierDataInMongoDB(MongoUrl url)
        {
            _client = new MongoClient(url);

            string dbName;
            if (string.IsNullOrWhiteSpace(url.DatabaseName))
                dbName = "RoninML";
            else
                dbName = url.DatabaseName;

            _db = _client.GetDatabase(dbName);
        }

        public ClassifierDataInMongoDB(string urlString)
            : this(new MongoUrl(urlString))
        {
        }

        public IEnumerable<C> CategoryKeys()
        {
            throw new NotImplementedException();
        }

        public long CountCategory(C cat)
        {
            throw new NotImplementedException();
        }

        public long CountFeature(F feat, C cat)
        {
            throw new NotImplementedException();
        }

        public void IncrementCategory(C cat)
        {
            throw new NotImplementedException();
        }

        public void IncrementFeature(F feat, C cat)
        {
            throw new NotImplementedException();
        }

        public long TotalCategoryItems()
        {
            throw new NotImplementedException();
        }

        public void RemoveCategory(C cat)
        {
            throw new NotImplementedException();
        }
    }
}
