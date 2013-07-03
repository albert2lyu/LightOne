using Common.Data;
using MongoDB.Driver;
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
    }
}
