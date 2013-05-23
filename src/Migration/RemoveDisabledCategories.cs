using Business;
using Common.Data;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migration {
    class RemoveDisabledCategories {
        public void Run() {
            var categoriesCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Category>("categories");
            var productsCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");
            productsCollection.EnsureIndex("CategoryIds");

            foreach (var category in categoriesCollection.Find(Query<Category>.EQ(c => c.Enable, false))) {
                Console.Write(category.Name);
                var result = productsCollection.Remove(Query<Product>.EQ(p => p.CategoryIds, category.Id));
                categoriesCollection.Remove(Query<Category>.EQ(c => c.Id, category.Id));
                Console.WriteLine(result.DocumentsAffected + 1);
            }
        }
    }
}
