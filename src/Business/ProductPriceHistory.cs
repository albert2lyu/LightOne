using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Business {
    public class ProductPriceHistory {
        public double Price { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public ProductPriceHistory() {
        }

        public ProductPriceHistory(double price, DateTime time) {
            Price = price;
            Time = time;
        }
    }
}
