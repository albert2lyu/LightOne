using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bee.Yhd
{
    class YhdDataSource {
        public async static Task<IEnumerable<Category>> ExtractCategories() {
            var extractor = new YhdCategoryExtractor();
            return await extractor.ExtractCategories();
        }

        public async static Task<IEnumerable<Product>> ExtractProductsInCategoryAsync(string categoryNumber) {
            var extractor = new YhdProductExtractor();
            return await extractor.ExtractProductsInCategoryAsync(categoryNumber);
        }
    }
}
