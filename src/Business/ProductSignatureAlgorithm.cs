using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Business {
    class ProductSignatureAlgorithm {
        public static Signature ComputeSignature(Product product) {
            if (product == null)
                throw new ArgumentNullException("product");

            var json = JsonConvert.SerializeObject(new {
                product.Number,
                product.Source,
                product.Name,
                product.Url,
                product.ImgUrl,
                product.Price,
                product.CategoryIds
            });

            return new Signature(Hash(json));
        }

        private static byte[] Hash(string input) {
            using (var hasher = MD5.Create()) {
                return hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }
    }
}
