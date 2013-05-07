using System;
using System.Collections.Generic;
using System.Linq;
using Business;
using Common.Data;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System.IO;

namespace Migration {
    
    class Program {

        private static IEnumerable<string> Read() {
            var productsCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");
            var products = productsCollection.Find(Query<Product>.Size(p => p.PriceHistory, 1));
            foreach (var p in products) {
                var json = JsonConvert.SerializeObject(new {
                    Id = p.Id,
                    PriceHistory = p.PriceHistory
                });
                yield return json;
            }
        }

        static void Main(string[] args) {
            //var index = 0;
            //foreach (var line in Read()) {

            //    File.AppendAllLines(
            //        string.Format("data-{0}.txt", (index++ / 10000).ToString("0")),
            //        new[] { line });
            //}
            //return;
            var count = 0;
            var productsCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");

            foreach (var i in Enumerable.Range(0, 51)) {
                foreach (var line in File.ReadAllLines("data-" + i + ".txt")) {
                    var item = JsonConvert.DeserializeAnonymousType(line,
                        new {
                            Id = string.Empty,
                            PriceHistory = new ProductPriceHistory[0]
                        });

                    //foreach (var item in data) {
                    if (count++ % 100 == 0)
                        Console.Write(".");

                    var product = productsCollection.FindOne(Query<Product>.EQ(p => p.Id, item.Id));
                    if (product != null) {
                        if (product.PriceHistory == null)
                            product.PriceHistory = new List<ProductPriceHistory>();
                        product.PriceHistory.AddRange(item.PriceHistory);
                        product.PriceHistory = RemoveDupItems(product.PriceHistory);
                        productsCollection.Save(product);
                    }
                }
            }
        }

        private static List<ProductPriceHistory> RemoveDupItems(List<ProductPriceHistory> items) {
            var result = new List<ProductPriceHistory>();

            var lastPrice = decimal.MinusOne;
            foreach (var item in items.OrderBy(p => p.Time)) {
                if (lastPrice == item.Price)
                    continue;

                lastPrice = item.Price;
                result.Add(item);
            }

            return result;
        }
    }
}
