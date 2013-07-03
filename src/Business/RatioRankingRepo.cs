using Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class RatioRankingRepo {
        private static readonly MongoCollection<RatioRanking> Collection;

        static RatioRankingRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<RatioRanking>("ratio_rankings");

            Collection.EnsureIndex(IndexKeys.Ascending("CategoryId"), IndexOptions.SetUnique(true));
        }

        public RatioRanking GetByCategoryId(string categoryId) {
            return Collection.FindOne(Query<RatioRanking>.EQ(p => p.CategoryId, categoryId));
        }

        public void Save(RatioRanking o) {
            Collection.Save(o);
        }
    }
}
