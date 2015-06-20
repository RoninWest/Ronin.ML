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
	[BsonIgnoreExtraElements(false, Inherited = true)]
	class FeatureCategoryKey<F, C>
		//where F : IComparable<F>
		//where C : IComparable<C>
		where F : IEquatable<F>
		where C : IEquatable<C>
	{
		[BsonRequired]
		[BsonElement("f")]
		public F Feature { get; set; }

		[BsonRequired]
		[BsonElement("c")]
		public C Category { get; set; }

		public override int GetHashCode()
		{
			int r = 0;
			if (Feature != null)
				r = Feature.GetHashCode() * 13;
			if (Category != null)
				r += Category.GetHashCode();

			return r;
		}

		public override bool Equals(object obj)
		{
			if(obj != null && obj is FeatureCategoryKey<F, C>)
			{
				var o = obj as FeatureCategoryKey<F, C>;
				if(Feature != null && o.Feature != null)
				{
					if(Category != null && o.Category != null)
						return Feature.Equals(o.Feature) && Category.Equals(o.Category);
				}
            }
			return false;
		}
	}
}
