using Business;
using Common.Data;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migration {
    class FixPrice {
        public void Run(decimal ratio) {
            var count = 1;

            while (true) {
                var productsCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");
                var products = productsCollection.Find(
                        Query<Product>.LT(p => p.ChangedRatio, 0))
                    .SetSortOrder(SortBy<Product>.Descending(p => p.ChangedRatio))
                    .SetLimit(100);

                foreach (var p in products) {
                    Console.WriteLine("[{2}]{0} -> {1}", p.OldPrice, p.Price, count++);
                    if (p.PriceHistory == null) {
                        productsCollection.Remove(Query<Product>.EQ(x => x.Id, p.Id));
                        return;
                    }

                    var trimmedPriceHistory = p.PriceHistory.Where(h => h.Price < p.Price * ratio && h.Price > p.Price / ratio).ToList();
                    if (trimmedPriceHistory.Count == 0) {
                        productsCollection.Remove(Query<Product>.EQ(x => x.Id, p.Id));
                        return;
                    }

                    p.PriceHistory = trimmedPriceHistory;
                    p.Price = trimmedPriceHistory.Last().Price;
                    p.OldPrice = trimmedPriceHistory[trimmedPriceHistory.Count - 2 >= 0 ? trimmedPriceHistory.Count - 2 : 0].Price;
                    var newRatio = CaclChangedRatio(p.OldPrice, p.Price);
                    if (newRatio != p.ChangedRatio) {
                        p.ChangedRatio = newRatio;
                        productsCollection.Save(p);
                    }
                }
            }
        }

        private static decimal CaclChangedRatio(decimal oldPrice, decimal newPrice) {
            if (oldPrice == 0)
                return 0;
            return (newPrice - oldPrice) / oldPrice;
        }
    }
}