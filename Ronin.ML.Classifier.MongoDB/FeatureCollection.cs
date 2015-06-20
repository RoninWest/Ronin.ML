using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq.Utils;

namespace Ronin.ML.Classifier
{
	class FeatureCollection<F, C>
		where F : IEquatable<F>
		where C : IEquatable<C>
	{
		readonly IMongoCollection<FeatureCountItem<F, C>> _col;

		public FeatureCollection(IMongoDatabase db, string colName = "feature")
		{
			if (db == null)
				throw new ArgumentNullException("db");
			if (string.IsNullOrWhiteSpace(colName))
				throw new ArgumentOutOfRangeException("colName can not be null or blank");

			_col = db.GetCollection<FeatureCountItem<F, C>>(colName.ToLower());
		}

		public virtual async Task<long> CountFeature(F feat, C cat)
		{
			var k = new FeatureCategoryKey<F, C> { Feature = feat, Category = cat };

			//var f = new FilterDefinitionBuilder<FeatureCountItem<F, C>>();
			//var res = _col.Find(f.Eq(x => x.Id, k)).FirstOrDefaultAsync();

			var res = _col.Find(o => o.Id.Equals(k)).FirstOrDefaultAsync();
			return await res.ContinueWith(o =>
			{
				return o.Result != null ? o.Result.Value : 0;
			});
		}

		public virtual async void IncrementFeature(F feat, C cat)
		{
			var f = new FilterDefinitionBuilder<FeatureCountItem<F, C>>();
			var ub = new UpdateDefinitionBuilder<FeatureCountItem<F, C>>();

			var op = new FindOneAndUpdateOptions<FeatureCountItem<F, C>>();
			op.IsUpsert = true;
			op.ReturnDocument = ReturnDocument.Before;
			//op.Sort = new SortDefinitionBuilder<FeatureCountItem<F, C>>().Ascending(o => o.Id);

			var k = new FeatureCategoryKey<F, C> { Feature = feat, Category = cat };
			await _col.FindOneAndUpdateAsync(f.Eq(o => o.Id, k), ub.Inc(o => o.Value, 1), op);
		}

		public virtual async void RemoveCategory(C cat)
		{
			var f = new FilterDefinitionBuilder<FeatureCountItem<F, C>>();
			await _col.DeleteManyAsync(f.Eq(o => o.Id.Category, cat));
		}
    }
} 