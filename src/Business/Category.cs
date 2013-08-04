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
        public ObjectId Id { get; set; }

        public string Source { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string ParentNumber { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public int Sort { get; set; }
    }
}
