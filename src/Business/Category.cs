using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using MongoDB.Driver;

namespace Business
{
    public class Category {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Source { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ParentNumber { get; set; }

        public int Level { get; set; }

        public int StableTimes { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }

        public int Sort { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ProductsUpdateTime { get; set; }

        public bool Enable { get; set; }

        public Category() {
            Level = 1;
            Enable = true;
        }

        private static Category GetBySourceAndNumber(string source, string number) {
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .FindOne(Query.And(Query<Category>.EQ(c => c.Source, source), Query<Category>.EQ(c => c.Number, number)));
        }

        public bool NeedReExtract() {
            return Level == 3
                && ProductsUpdateTime + TimeSpan.FromHours(StableTimes) < DateTime.Now;
                //&& GetAncestorCategories(Id).All(c => c.Number != "123" && c.Number != "25228");
        }

        

        

        

        

        public static Category Get(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .FindOne(Query<Category>.EQ(c => c.Id, id));
        }

        public static IEnumerable<Category> GetAncestorCategories(string id) {
            var category = Get(id);
            if (category == null)
                yield break;

            var parent = GetBySourceAndNumber(category.Source, category.ParentNumber);
            if (parent != null) {
                foreach (var c in GetAncestorCategories(parent.Id))
                    yield return c;
            }
            yield return category;
        }

        public static void UpdateProductsUpdateTime(string id, DateTime productsUpdateTime) {
            DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .Update(Query<Category>.EQ(c => c.Id, id),
                    Update<Category>.Set(c => c.ProductsUpdateTime, productsUpdateTime));
        }

        public static void ResetStableTimes(string id) {
            DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .Update(Query<Category>.EQ(c => c.Id, id),
                    Update<Category>.Set(c => c.StableTimes, 0));
        }

        public static void IncreaseStableTimes(string id, int maxStableTimes = 24) {
            DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .Update(Query.And(
                    Query<Category>.EQ(c => c.Id, id),
                    Query<Category>.LT(c => c.StableTimes, maxStableTimes)),
                    Update<Category>.Inc(c => c.StableTimes, 1));
        }

        public static IEnumerable<Tuple<Category, IEnumerable<Category>>> GetCategoryTree(string categoryId) {
            var category = Get(categoryId);
            if (category == null) {
                // 分类不存在，返回顶级分类
                return GetByLevel(1).Select(c => new Tuple<Category, IEnumerable<Category>>(c, null));
            }

            // 获取下级分类
            var subCategories = GetBySourceAndParentNumber(category.Source, category.Number).ToList();
            var brotherCategories = GetBySourceAndParentNumber(category.Source, category.ParentNumber).ToList();
            if (subCategories.Count > 0) {
                // 有下级分类，返回兄弟分类及当前分类的下级分类
                return brotherCategories.Select(bc => new Tuple<Category, IEnumerable<Category>>(bc, bc.Id == categoryId ? subCategories : null));
            }

            // 无下级分类
            var parentCategory = GetBySourceAndNumber(category.Source, category.ParentNumber);
            if (parentCategory != null) {
                // 返回父分类的兄弟分类及当前分类的兄弟分类
                var parentBrotherCategories = GetBySourceAndParentNumber(parentCategory.Source, parentCategory.ParentNumber);
                return parentBrotherCategories.Select(pbc => new Tuple<Category, IEnumerable<Category>>(pbc, pbc.Number == category.ParentNumber ? brotherCategories : null));
            }
            else {
                // 没有父分类也没有子分类，返回当前分类的兄弟分类
                return brotherCategories.Select(bc => new Tuple<Category, IEnumerable<Category>>(bc, null));
            }
        }

        private static IEnumerable<Category> GetBySourceAndParentNumber(string source, string parentNumber) {
            return DatabaseFactory.CreateMongoDatabase()
                    .GetCollection<Category>("categories")
                    .Find(Query.And(Query<Category>.EQ(c => c.Source, source), Query<Category>.EQ(c => c.ParentNumber, parentNumber)))
                    .SetSortOrder(SortBy<Category>.Ascending(c => c.Sort));
        }

        private static IEnumerable<Category> GetByLevel(int level) {
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .Find(Query<Category>.EQ(c => c.Level, level))
                .SetSortOrder(SortBy<Category>.Ascending(c => c.Sort));
        }

        public static IEnumerable<Category> GetByIds(string[] ids) {
            if (ids == null || ids.Length == 0)
                return null;
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .Find(Query<Category>.In(p => p.Id, ids));
        }
    }
}
