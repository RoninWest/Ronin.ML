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
        where C : IComparable<C>
    {
        readonly IMongoCollection<CategoryItem<C>> _col;

        public CategoryCollection(IMongoDatabase db, string colName = "Category")
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (string.IsNullOrWhiteSpace(colName))
                throw new ArgumentOutOfRangeException("colName can not be null or blank");

            _col = db.GetCollection<CategoryItem<C>>(colName);
        }

        public IEnumerable<C> CategoryKeys()
        {
            var f = new BsonDocumentFilterDefinition<CategoryItem<C>>(new BsonDocument());
            var p = new ProjectionDefinitionBuilder<CategoryItem<C>>();
            var op = new FindOptions<CategoryItem<C>, C>
            {
                Projection = p.Include(o => o.Id),
            };
            var res = _col.FindAsync<C>(filter: f, options: op).GetAwaiter().GetResult();
            return res.Current;
        }

        public long CountCategory(C cat)
        {
            var f = new FilterDefinitionBuilder<CategoryItem<C>>();
            var p = new ProjectionDefinitionBuilder<CategoryItem<C>>();
            var res = _col.FindAsync(filter: f.Eq(o => o.Id, cat)).GetAwaiter().GetResult();
            return res.Current.FirstOrDefault().Value;
        }
    }
}
