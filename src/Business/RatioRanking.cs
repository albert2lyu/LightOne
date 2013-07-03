using Common.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    /// <summary>
    /// 折扣排行
    /// </summary>
    public class RatioRanking {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public string CategoryId { get; set; }

        public string[] ProductIds { get; set; }
    }
}
