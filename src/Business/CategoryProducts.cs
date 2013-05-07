using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class CategoryProducts {
        public string CategoryId { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public virtual void Upsert() {
            var ancestorCategoryIds = Category.GetAncestorCategories(CategoryId).Select(c => c.Id).ToArray();

            var hasChanges = false;

            foreach (var product in Products) {
                product.CategoryIds = ancestorCategoryIds;
                hasChanges |= product.Upsert();
            }

            // 设置分类下产品的更新时间
            Category.UpdateProductsUpdateTime(CategoryId, DateTime.Now);

            // 设置分类下产品的稳定次数
            if (hasChanges)
                Category.ResetStableTimes(CategoryId);
            else
                Category.IncreaseStableTimes(CategoryId);
        }
    }
}
