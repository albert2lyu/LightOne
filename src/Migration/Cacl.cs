using Business;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migration {
    class Cacl {
        private IEnumerable<Category> GetAllCategories(){
            return DatabaseFactory.CreateMongoDatabase()
                .GetCollection<Category>("categories")
                .FindAll();
        }

        public void Run() {
            RankCategory(null);
            foreach (var category in GetAllCategories()) {
                Console.WriteLine(string.Format("{0:HH:mm:ss} {1}", DateTime.Now, category.Name));
                RankCategory(category);
            }
        }

        private static void RankCategory(Category category) {
            var categoryId = category != null ? category.Id : null;
            
            var products = Product.GetByPriceReduced(categoryId, 150, 24);

            var repo = new RatioRankingRepo();
            var ranking = repo.GetByCategoryId(categoryId);
            if (ranking == null) {
                ranking = new RatioRanking();
                ranking.CategoryId = categoryId;
            }
            ranking.ProductIds = products.Select(p => p.Id).ToArray();
            ranking.UpdateTime = DateTime.Now;

            repo.Save(ranking);
        }
    }
}
