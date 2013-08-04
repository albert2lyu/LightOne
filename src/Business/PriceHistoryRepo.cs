using Common.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class PriceHistoryRepo {
        public static readonly MongoCollection<PriceHistory> Collection;

        static PriceHistoryRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<PriceHistory>("price_history");
        }

        public PriceHistory Get(ObjectId id) {
            return Collection.FindOne(Query<PriceHistory>.EQ(_ => _.Id, id));
        }

        public double? GetPriceInDay(ObjectId id, DateTime day) {
            var prices = GetPricesInDay(id, day);
            if (prices.Length > 0)
                return prices[0];
            return null;
        }

        public double[] GetPricesInDay(ObjectId id, DateTime day) {
            var priceHistory = Get(id);
            if (priceHistory == null || priceHistory._ == null || priceHistory._.Count == 0)
                return new double[0];

            // 查找当日内发生变化的价格
            var start = day.Date;
            var finish = start.AddDays(1);
            var prices = priceHistory._.Where(p => p.Time >= start && p.Time < finish).ToList();
            if (prices.Count > 0)
                return prices.OrderBy(p => p.Time).Select(p => p.Price).ToArray();

            // 当日内价格没有发生变化，查询之前的最新价格
            var newestPrice = priceHistory._
                .Where(p => p.Time < start)
                .OrderBy(p => p.Time)
                .LastOrDefault();

            if (newestPrice == null)
                return new double[0];
            else
                return new double[] { newestPrice.Price };
        }
    }
}
