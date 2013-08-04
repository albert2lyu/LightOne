using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;

namespace Business {
    public class Product {
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Number { get; set; }

        public string Source { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ImgUrl { get; set; }

        public double Price { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public ObjectId[] CategoryIds { get; set; }

        public double OldPrice { get; set; }

        public double ChangedRatio { get; set; }

        public string SubTitle { get; set; }

        public string Brand { get; set; }

        public static Product ParseAndGetByUrl(string url) {
            var source = string.Empty;
            var regSource = new Regex(@"(1mall)|(yihaodian)\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (regSource.IsMatch(url))
                source = "yhd";

            var number = string.Empty;
            var regNumber = new Regex(@"/(\d+)_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regNumber.Match(url);
            if (m.Success)
                number = m.Groups[1].Value;

            var productRepo = new ProductRepo();
            return productRepo.GetBySourceAndNumber(source, number);
        }

        
    }
}
