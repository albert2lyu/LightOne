using Common.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    public class CategoryRepo {
        private static readonly MongoCollection<Category> Collection;

        static CategoryRepo() {
            Collection = DatabaseFactory.CreateMongoDatabase().GetCollection<Category>("categories");

            //Collection.EnsureIndex(IndexKeys.Ascending("CategoryId"), IndexOptions.SetUnique(true));
        }

        public IEnumerable<Category> GetAll() {
            return Collection.FindAll();
        }

        public void Save(Category category) {
            Collection.Save(category);
        }

        public IEnumerable<Category> GetBySource(string source) {
            return Collection.Find(Query<Category>.EQ(c => c.Source, source));
        }

        public void DisableById(string categoryId) {
            Collection.Update(Query<Category>.EQ(c => c.Id, categoryId), Update<Category>.Set(c => c.Enable, false));
        }
    }
}
