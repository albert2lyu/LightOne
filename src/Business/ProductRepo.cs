using Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class ProductRepo {
        private static readonly MongoCollection<Product> Collection;

        static ProductRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");

            Collection.EnsureIndex(IndexKeys.Ascending("CategoryIds"));
            //Collection.EnsureIndex(IndexKeys.Ascending("Source", "Number"), IndexOptions.SetUnique(true));
        }

        public IEnumerable<Product> GetByCategoryId(string categoryId) {
            if (string.IsNullOrWhiteSpace(categoryId))
                return new Product[0];

            return Collection.Find(Query<Product>.EQ(p => p.CategoryIds, categoryId));
        }
    }
}
