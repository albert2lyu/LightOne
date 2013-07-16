using Common.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class RatioRankingRepo {
        public static readonly MongoCollection<RatioRanking> Collection;

        static RatioRankingRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<RatioRanking>("ratio_rankings");
        }

        public RatioRanking GetByCategoryId(ObjectId categoryId) {
            return Collection.FindOne(Query<RatioRanking>.EQ(p => p.CategoryId, categoryId));
        }

        public void Upsert(ObjectId categoryId, ObjectId[] productIds) {
            var ranking = GetByCategoryId(categoryId);
            if (ranking == null) {
                ranking = new RatioRanking();
                ranking.CategoryId = categoryId;
            }
            ranking.ProductIds = productIds;
            ranking.UpdateTime = DateTime.Now;

            Collection.Save(ranking);
        }
    }
}
