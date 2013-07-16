using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using System.Text.RegularExpressions;

namespace Business {
    public class Product {
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Number { get; set; }

        public string Source { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ImgUrl { get; set; }

        public double Price { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public ObjectId[] CategoryIds { get; set; }

        public double OldPrice { get; set; }

        public double ChangedRatio { get; set; }

        public List<ProductPriceHistory> PriceHistory { get; set; }

        public Signature Signature { get; set; }


        private static double CaclChangedRatio(double oldPrice, double newPrice) {
            if (oldPrice == 0)
                return 0;
            return (newPrice - oldPrice) / oldPrice;
        }

        public virtual bool Upsert() {
            // 计算当前产品信息的签名
            //var currentSignature = ProductSignatureAlgorithm.ComputeSignature(this);
            //// 如果签名在缓存中已经存在，表示此产品信息未发生变化，无需处理
            //var container = new ProductSignatureContainer();
            //if (container.Contains(Source, Number, currentSignature))
            //    return false;

            // 获取已经存在的产品
            var productRepo = new ProductRepo();
            var existsProduct = productRepo.GetBySourceAndNumber(Source, Number);

            if (existsProduct != null) {
                Id = existsProduct.Id;
                CreateTime = existsProduct.CreateTime;
                UpdateTime = DateTime.Now;
                OldPrice = existsProduct.Price;
                PriceHistory = existsProduct.PriceHistory;

                // 价格发生变化，计算调价比例
                if (OldPrice != Price) {
                    ChangedRatio = CaclChangedRatio(OldPrice, Price);
                    if (PriceHistory == null)
                        PriceHistory = new List<ProductPriceHistory>();
                    PriceHistory.Add(new ProductPriceHistory(Price, DateTime.Now));
                }
            }
            else {
                CreateTime = DateTime.Now;
                UpdateTime = DateTime.Now;
                OldPrice = Price;
                ChangedRatio = 0;

                PriceHistory = new List<ProductPriceHistory>();
                PriceHistory.Add(new ProductPriceHistory(Price, DateTime.Now));
            }

            DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Product>("products")
                .Save(this);

            // 返回值：价格是否发生变化（新品或调价返回true），而不是产品的其他属性是否发生变化
            return existsProduct == null || existsProduct.Price != Price;
        }

        

        public static Product GetById(ObjectId id) {
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Product>("products")
                .FindOne(Query<Product>.EQ(p => p.Id, id));
        }

        public double? GetPriceInDay(DateTime day) {
            var prices = GetPricesInDay(day);
            if (prices.Length > 0)
                return prices[0];
            return null;
        }

        public double[] GetPricesInDay(DateTime day) {
            if (PriceHistory == null || PriceHistory.Count == 0)
                return new double[0];

            // 查找当日内发生变化的价格
            var start = day.Date;
            var finish = start.AddDays(1);
            var prices = PriceHistory.FindAll(p => p.Time >= start && p.Time < finish);
            if (prices.Count > 0)
                return prices.OrderBy(p => p.Time).Select(p => p.Price).ToArray();

            // 当日内价格没有发生变化，查询之前的最新价格
            var newestPrice = PriceHistory
                .Where(p => p.Time < start)
                .OrderBy(p => p.Time)
                .LastOrDefault();

            if (newestPrice == null)
                return new double[0];
            else
                return new double[] { newestPrice.Price };
        }

        public static Product ParseAndGetByUrl(string url) {
            var source = string.Empty;
            var regSource = new Regex(@"(1mall)|(yihaodian)\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (regSource.IsMatch(url))
                source = "yhd";

            var number = string.Empty;
            var regNumber = new Regex(@"/(\d+)_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regNumber.Match(url);
            if (m.Success)
                number = m.Groups[1].Value;

            var productRepo = new ProductRepo();
            return productRepo.GetBySourceAndNumber(source, number);
        }

        
    }
}
