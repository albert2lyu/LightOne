using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Business {
    public class ProductSignature {
        public string Source { get; set; }

        public string Number { get; set; }

        public Signature Signature { get; set; }

        public static Signature ComputeSignature(Product product) {
            if (product == null)
                throw new ArgumentNullException("product");

            var json = JsonConvert.SerializeObject(new {
                product.Number,
                product.Source,
                product.Name,
                product.Url,
                product.ImgUrl,
                product.Price
            });

            using (var hasher = MD5.Create()) {
                return new Signature(hasher.ComputeHash(Encoding.UTF8.GetBytes(json)));
            }
        }
    }
}
