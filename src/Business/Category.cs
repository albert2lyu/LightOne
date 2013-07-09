using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace Business
{
    public class Category {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Source { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ParentNumber { get; set; }

        public int Level { get; set; }

        public int StableTimes { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public int Sort { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ProductsUpdateTime { get; set; }

        public bool Enable { get; set; }

        public Category() {
            Level = 1;
            Enable = true;
        }
    }
}
