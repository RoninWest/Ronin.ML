using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Ronin.ML.Classifier
{
    [BsonIgnoreExtraElements(true, Inherited = true)]
    class FeatureCountItem<F, C>
        where F : IComparable<F>
        where C : IComparable<C>
    {
        [BsonId]
        public F Id { get; set; }

        [BsonIgnore]
        Dictionary<C, long> _map = new Dictionary<C, long>();

        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<C, long> Map
        {
            get { return _map; }
            set { _map = value ?? new Dictionary<C, long>(); }
        }
    }
}
