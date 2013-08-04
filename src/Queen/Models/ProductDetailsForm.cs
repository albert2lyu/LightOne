using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Queen.Models
{
    public class ProductDetailsForm {
        public readonly Product Product;
        public readonly IEnumerable<Category> Categories;

        public ProductDetailsForm(Product product, IEnumerable<Category> categories) {
            if (product == null)
                throw new ArgumentNullException("product");

            Product = product;

            if (categories != null)
                Categories = categories.OrderBy(c => c.Sort).ToList();
            else
                Categories = new Category[0];
        }

        public string Title {
            get {
                var categoryNames = string.Empty;
                if (Categories != null)
                    categoryNames = string.Join(" ", Categories.Reverse().Select(c => c.Name));
                return Product.Name + (!string.IsNullOrWhiteSpace(categoryNames) ? " " + categoryNames : string.Empty);
            }
        }

        public string ImgUrl {
            get {
                var smallImgUrl = Product.ImgUrl;
                var regex = new Regex(@"\d{3}x\d{3}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return regex.Replace(smallImgUrl, "380x380");
            }
        }
    }
}