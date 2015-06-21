using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ronin.ML.Classifier
{
    [BsonIgnoreExtraElements(true, Inherited = true)]
    class CategoryItem<C>
    {
        [BsonId]
        public C Id { get; set; }

		[BsonDefaultValue(0)]
		[BsonRequired]
		[BsonElement("v")]
        public long Value { get; set; }
    }
}
