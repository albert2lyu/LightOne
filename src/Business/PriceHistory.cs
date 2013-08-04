using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Business {
    public class PriceHistory {
        public ObjectId Id { get; set; }

        public IList<PriceHistoryData> _ { get; set; }

        public class PriceHistoryData {
            [BsonElement("price")]
            public double Price { get; set; }

            [BsonElement("time")]
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime Time { get; set; }
        }

    }
}