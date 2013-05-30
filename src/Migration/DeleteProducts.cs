using Business;
using Common.Data;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migration {
    class DeleteProducts {
        public void Run(int days) {
            var productsCollection = DatabaseFactory.CreateMongoDatabase().GetCollection<Product>("products");
            productsCollection.Remove(Query<Product>.LT(p => p.UpdateTime, DateTime.Now.AddDays(-days)));
            Console.WriteLine("done");
        }
    }
}
