using System;
using System.Collections.Generic;
using System.Linq;

namespace Bee.Yhd
{
    class YhdDataSource {
        public IEnumerable<CategoryProxy> ExtractCategories() {
            var extractor = new YhdCategoryExtractor();
            return extractor.ExtractCategories();
        }

        public IEnumerable<ProductProxy> ExtractProductsInCategory(string categoryNumber) {
            var extractor = new YhdProductExtractor();
            return extractor.ExtractProductsInCategory(categoryNumber);
        }
    }
}
