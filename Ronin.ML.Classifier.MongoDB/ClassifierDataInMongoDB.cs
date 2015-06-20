﻿using System;
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
		//where F : IComparable<F>
		//where C : IComparable<C>
		where F : IEquatable<F>
		where C : IEquatable<C>
	{
        readonly IMongoClient _client;
        readonly IMongoDatabase _db;
		readonly FeatureCollection<F, C> _fc;
		readonly CategoryCollection<C> _cc;

		public ClassifierDataInMongoDB(string urlString)
			: this(new MongoUrl(urlString))
		{
		}

		public ClassifierDataInMongoDB(MongoUrl url, string featureCollection = null, string categoryCollection = null)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            _client = new MongoClient(url);

            string dbName;
            if (string.IsNullOrWhiteSpace(url.DatabaseName))
                dbName = "RoninML";
            else
                dbName = url.DatabaseName;

            _db = _client.GetDatabase(dbName.ToLower());

			_fc = new FeatureCollection<F, C>(_db, featureCollection);
			_cc = new CategoryCollection<C>(_db, categoryCollection);
        }

        public IEnumerable<C> CategoryKeys()
        {
			return _cc.CategoryKeys().GetAwaiter().GetResult();
        }

        public long CountCategory(C cat)
        {
			return _cc.CountCategory(cat).GetAwaiter().GetResult();
        }

        public long CountFeature(F feat, C cat)
        {
			return _fc.CountFeature(feat, cat).GetAwaiter().GetResult();
        }

        public void IncrementCategory(C cat)
        {
			_cc.IncrementCategory(cat);
        }

        public void IncrementFeature(F feat, C cat)
        {
			_fc.IncrementFeature(feat, cat);
        }

        public long TotalCategoryItems()
        {
			return _cc.TotalCategoryItems().GetAwaiter().GetResult();
        }

        public void RemoveCategory(C cat)
        {
			_cc.RemoveCategory(cat);
			_fc.RemoveCategory(cat);
        }
    }
}
