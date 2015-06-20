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
    class CategoryCollection<C>
		//where C : IComparable<C>
		where C : IEquatable<C>
	{
        readonly IMongoCollection<CategoryItem<C>> _col;

        public CategoryCollection(IMongoDatabase db, string colName = "category")
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (string.IsNullOrWhiteSpace(colName))
                throw new ArgumentOutOfRangeException("colName can not be null or blank");

            _col = db.GetCollection<CategoryItem<C>>(colName.ToLower());
        }

        public virtual async Task<IEnumerable<C>> CategoryKeys()
        {
			//var f = new BsonDocumentFilterDefinition<CategoryItem<C>>(new BsonDocument());
			//return await _col.Find(f).Project(o => o.Id).ToListAsync();

			return await _col.Find(o => true).Project(o => o.Id).ToListAsync();
        }

        public virtual async Task<long> CountCategory(C cat)
        {
			//var f = new FilterDefinitionBuilder<CategoryItem<C>>();
			//return await _col.Find(f.Eq(o => o.Id, cat)).Project(o => o.Value).FirstOrDefaultAsync();

			return await _col.Find(o => o.Id.Equals(cat)).Project(o => o.Value).FirstOrDefaultAsync();
		}

		public virtual async Task<long> TotalCategoryItems()
		{
			return await _col.Aggregate().Group(o => true, g => g.Sum(o => o.Value)).FirstOrDefaultAsync();
		}

		public virtual async void IncrementCategory(C cat)
		{
			var f = new FilterDefinitionBuilder<CategoryItem<C>>();

			var ub = new UpdateDefinitionBuilder<CategoryItem<C>>();

			var op = new FindOneAndUpdateOptions<CategoryItem<C>>();
			op.IsUpsert = true;
			op.ReturnDocument = ReturnDocument.Before;
			//op.Sort = new SortDefinitionBuilder<CategoryItem<C>>().Ascending(o => o.Id);

            await _col.FindOneAndUpdateAsync(f.Eq(o => o.Id, cat), ub.Inc(o => o.Value, 1), op);
		}

		public virtual async void RemoveCategory(C cat)
		{
			await _col.DeleteOneAsync(o => o.Id.Equals(cat));

			//long val;
			//if (_cc.TryRemove(cat, out val))
			//{
			//	F[] features = (from p in _fc
			//					where p.Value.ContainsKey(cat)
			//					select p.Key).ToArray();
			//	FeatureCount<C> fc;
			//	features.ForEach(f => _fc.TryRemove(f, out fc));
			//}
		}
	}
}
