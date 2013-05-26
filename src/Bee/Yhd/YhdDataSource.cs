using Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bee.Yhd
{
    class YhdDataSource {
        public static IEnumerable<Category> ExtractCategories() {
            var extractor = new YhdCategoryExtractor();
            return extractor.ExtractCategories();
        }

        public static IEnumerable<Product> ExtractProductsInCategory(string categoryNumber) {
            var extractor = new YhdProductExtractor();
            return extractor.ExtractProductsInCategory(categoryNumber);
        }
    }
}
