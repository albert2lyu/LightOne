using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Business;
using Common;
using Common.Data;
using MongoDB.Driver.Builders;

namespace Migration {
    
    class Program {

        static void Main(string[] args) {

            var sw = new Stopwatch();
            sw.Start();

            var from = DatabaseFactory.CreateMongoDatabase();
            var to = DatabaseFactory.CreateMongoDatabase("light-1.com");

            if (false) {
                var categoryCollection = to.GetCollection<Category>("categories");
                categoryCollection.Drop();
                categoryCollection.EnsureIndex(IndexKeys.Ascending("Source", "Number"), IndexOptions.SetUnique(true));

                var categories = from.GetCollection<Category>("categories").FindAll();
                categoryCollection.InsertBatch(categories);
            }

            var productCollection = to.GetCollection<Product>("products");
            productCollection.Drop();
            
            var products = from.GetCollection<Product>("products").FindAll().ToList();

            var productsCollection = new List<List<Product>>();
            const int BATCH_SIZE = 200;
            for (var offset = 0; offset < products.Count; offset += BATCH_SIZE) {
                Console.Write(".");
                var productsInBatch = products.Skip(offset).Take(BATCH_SIZE).ToList();
                productsCollection.Add(productsInBatch);
            }

            var failedProducts = new ConcurrentBag<Product>();

            Parallel.ForEach(productsCollection,
                //new ParallelOptions { MaxDegreeOfParallelism = 4 },
                productsInBatch => {
                    try {
                        Console.Write("+");
                        productCollection.InsertBatch(productsInBatch);
                    }
                    catch {
                        Console.Write("x");
                        foreach (var p in productsInBatch)
                            failedProducts.Add(p);
                    }
                });

            for (var offset = 0; offset < failedProducts.Count; offset += BATCH_SIZE) {
                Console.Write("=");
                var productsInBatch = failedProducts.Skip(offset).Take(BATCH_SIZE).ToList();
                productCollection.InsertBatch(productsInBatch);
            }

            productCollection.EnsureIndex(IndexKeys.Ascending("Source", "Number"), IndexOptions.SetUnique(true));
        }

        

        private static void MigrateTable(string table, IDatabase from, IDatabase to) {
            var count = 0;
            var sql = string.Format("select * from {0}", table);
            from.ExecuteDataReader(sql, null, dr => {
                var item = new Dictionary<string, object>();
                foreach (var i in Enumerable.Range(0, dr.FieldCount)) {
                    var fieldName = dr.GetName(i);
                    var fieldValue = dr[i];
                    item.Add(fieldName, fieldValue);
                }

                var insertSql = string.Format(@"insert into {0}({1})values({2})",
                    table,
                    string.Join(", ", item.Keys.Select(p => string.Format(@"{0}", p))),
                    string.Join(", ", item.Keys.Select(p => "@" + p)));
                to.ExecuteNonQuery(insertSql, item);

                if (count++ % 1000 == 0)
                    Console.Write(".");
            });
        }
    }
}
