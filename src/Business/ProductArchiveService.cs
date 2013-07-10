using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class ProductArchiveService {
        private readonly CategoryRepo _CategoryRepo = new CategoryRepo();

        public void Archive(string categoryId, IEnumerable<Product> products) {
            var ancestorCategoryIds = _CategoryRepo.GetAncestorCategories(categoryId).Select(c => c.Id).ToArray();

            var hasChanges = false;

            foreach (var product in products) {
                product.CategoryIds = ancestorCategoryIds;
                hasChanges |= product.Upsert();
            }

            // 设置分类下产品的更新时间
            _CategoryRepo.UpdateProductsUpdateTime(categoryId, DateTime.Now);

            // 设置分类下产品的稳定次数
            if (hasChanges)
                _CategoryRepo.ResetStableTimes(categoryId);
            else
                _CategoryRepo.IncreaseStableTimes(categoryId);
        }
    }
}
