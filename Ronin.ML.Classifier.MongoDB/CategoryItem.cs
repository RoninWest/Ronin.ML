﻿using System;
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
		//where C : IComparable<C>
		where C : IEquatable<C>
    {
        [BsonId]
        public C Id { get; set; }

        [BsonElement("v")]
        public long Value { get; set; }
    }
}
