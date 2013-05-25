using Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bee
{
    class ProductComparer : IEqualityComparer<Product> {
        // ProductProxys are equal if their names and ProductProxy numbers are equal. 
        public bool Equals(Product x, Product y) {

            //Check whether the compared objects reference the same data. 
            if (object.ReferenceEquals(x, y))
                return true;

            //Check whether any of the compared objects is null. 
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            //Check whether the ProductProxys' properties are equal. 
            return x.Source == y.Source && x.Number == y.Number;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(Product product) {
            //Check whether the object is null 
            if (object.ReferenceEquals(product, null))
                return 0;

            //Calculate the hash code for the ProductProxy. 
            return (product.Source == null ? 0 : product.Source.GetHashCode()) ^
                (product.Number == null ? 0 : product.Number.GetHashCode());
        }
    }
}