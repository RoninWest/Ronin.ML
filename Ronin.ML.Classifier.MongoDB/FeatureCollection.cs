using System;
using System.Collections.Generic;
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
			//var f = new FilterDefinitionBuilder<FeatureCountItem<F, C>>();
			//var res = _col.Find(f.Eq(x => x.Id, feat)).FirstOrDefaultAsync();

			var res = _col.Find(o => o.Id.Equals(feat)).FirstOrDefaultAsync();
			return await res.ContinueWith(o =>
			{
				var fc = o.Result;
				long r = 0;
				if (fc != null && fc.Map != null)
					fc.Map.TryGetValue(cat, out r);

				return r;
			});
		}

		public virtual void IncrementFeature(F feat, C cat)
		{
			throw new NotImplementedException();
		}

		public virtual void RemoveCategory(C cat)
		{
			throw new NotImplementedException();
		}
    }
} 