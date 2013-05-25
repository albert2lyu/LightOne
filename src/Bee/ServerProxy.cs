using System;
using System.Collections.Generic;
using System.Linq;
using Business;

namespace Bee {
    class ServerProxy {
        public static IEnumerable<Category> UpsertCategories(IEnumerable<Category> categories) {
            return ProxyHelper.Post<IEnumerable<Category>>("Categories/Upsert", categories);
        }

        public static void UpsertProducts(string categoryId, IEnumerable<Product> products) {
            ProxyHelper.Post<object>("CategoryProducts/Upsert", new {
                CategoryId = categoryId,
                Products = products
            });
        }

        public static IEnumerable<ProductSignature> GetProductSignaturesByCategoryId(string categoryId) {
            return ProxyHelper.Get<IEnumerable<ProductSignature>>("Products/GetSignaturesByCategoryId?categoryId={0}", categoryId);
        }
    }
}