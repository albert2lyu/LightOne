using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business {
    public class RatioRankingService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CategoryRepo _CategoryRepo = new CategoryRepo();
        private readonly RatioRankingRepo _RatioRankingRepo = new RatioRankingRepo();

        public void RankAllCategories() {
            RankCategory(null);
            foreach (var category in _CategoryRepo.GetAll())
                RankCategory(category);
        }

        private void RankCategory(Category category) {
            var sw = new Stopwatch();
            sw.Start();

            var categoryId = category != null ? category.Id : null;
            var products = Product.GetByPriceReduced(categoryId, 150, 24);

            var ranking = _RatioRankingRepo.GetByCategoryId(categoryId);
            if (ranking == null) {
                ranking = new RatioRanking();
                ranking.CategoryId = categoryId;
            }
            ranking.ProductIds = products.Select(p => p.Id).ToArray();
            ranking.UpdateTime = DateTime.Now;

            _RatioRankingRepo.Save(ranking);
            Logger.DebugFormat("排序“{0}”价格，用时{1}", category != null ? category.Name : "[全部]", sw.Elapsed);
        }
    }
}
