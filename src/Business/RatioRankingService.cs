using Common.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business {
    public class RatioRankingService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly RatioRankingRepo _RatioRankingRepo = new RatioRankingRepo();
        private readonly ProductRepo _ProductRepo = new ProductRepo();

        public void RankAllCategories() {
            const int MAX_COUNT = 150;
            const int HOURS_AGO = 10;
            // 计算所有商品的折扣排行
            var products = _ProductRepo.GetByPriceReduced(ObjectId.Empty, MAX_COUNT, HOURS_AGO);
            _RatioRankingRepo.Upsert(ObjectId.Empty, products.Select(p => p.Id).ToArray());

            RankProductCategories(MAX_COUNT, HOURS_AGO);
        }

        private void RankProductCategories(int count, int hoursAgo) {
            // 逐个分类计算太慢了，所以改为aggregation实现。
            // ChangedRatio < 0 and UpdateTime > xxx order by ChangedRatio limit 150
            var match = new BsonDocument {{ 
                "$match", 
                new BsonDocument {
                    {"ChangedRatio", new BsonDocument {{"$lt", 0}}},
                    {"UpdateTime", new BsonDocument {{"$gt", DateTime.Now.AddHours(-hoursAgo)}}}
                }
            }};
            var sort = new BsonDocument {{ 
                "$sort", 
                new BsonDocument {{"ChangedRatio", 1}}
            }};
            var unwind = new BsonDocument { { "$unwind", "$CategoryIds" } };
            var group = new BsonDocument {{"$group", new BsonDocument { 
                                { "_id", "$CategoryIds"}, 
                                {"products", new BsonDocument{{ "$push", "$_id" }}} 
            }}};
            //var project = new BsonDocument {{ 
            //    "$project", 
            //    new BsonDocument { 
            //        {"_id", 0},
            //        {"CategoryId", "$_id"}, 
            //        {"ProductIds","$products"}
            //    } 
            //}};

            var pipeline = new[] { match, sort, unwind, group };

            var result = ProductRepo.Collection.Aggregate(pipeline);

            foreach (var doc in result.ResultDocuments) {
                var categoryId = doc["_id"].AsObjectId;
                var productIds = doc["products"].AsBsonArray.Select(id => id.AsObjectId).Take(count).ToArray();

                _RatioRankingRepo.Upsert(categoryId, productIds);
            }
        }
    }
}
