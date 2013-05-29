using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business {
    class CategoryEqualityComparer : IEqualityComparer<Category> {
        public bool Equals(Category x, Category y) {
            //Check whether the compared objects reference the same data. 
            if (object.ReferenceEquals(x, y))
                return true;

            //Check whether any of the compared objects is null. 
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Source == y.Source && x.Number == y.Number;
        }

        public int GetHashCode(Category obj) {
            //Check whether the object is null 
            if (object.ReferenceEquals(obj, null))
                return 0;

            //Calculate the hash code for the Category. 
            return 13 * (obj.Source == null ? 0 : obj.Source.GetHashCode()) +
                17 * (obj.Number == null ? 0 : obj.Number.GetHashCode());
        }
    }
}
