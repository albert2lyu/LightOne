using Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class ProductRepo {
        public static readonly MongoCollection<Product> Collection;

        static ProductRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");
        }

        public IEnumerable<Product> GetByCategoryId(string categoryId) {
            if (string.IsNullOrWhiteSpace(categoryId))
                return new Product[0];

            return Collection.Find(Query<Product>.EQ(p => p.CategoryIds, categoryId));
        }

        public Product GetBySourceAndNumber(string source, string number) {
            return Collection.FindOne(Query.And(
                Query<Product>.EQ(p => p.Source, source),
                Query<Product>.EQ(p => p.Number, number))
            );
        }

        /// <summary>
        /// 获取价格下调幅度最大的商品
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetByPriceReduced(int count, int hoursAgo) {
            return Collection.Find(
                    Query.And(
                        Query<Product>.LT(p => p.ChangedRatio, 0),
                        Query<Product>.GT(p => p.UpdateTime, DateTime.Now.AddHours(-hoursAgo))))
                .SetSortOrder(SortBy<Product>.Ascending(p => p.ChangedRatio))
                .SetLimit(count);
        }
    }
}
