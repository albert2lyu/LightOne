using System;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Business {
    public class ProductPriceHistory {
        public decimal Price { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public ProductPriceHistory() {
        }

        public ProductPriceHistory(decimal price, DateTime time) {
            Price = price;
            Time = time;
        }
    }
}
