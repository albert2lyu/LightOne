using System;
using System.Collections.Generic;
using System.Linq;

namespace Bee
{
    class ProductProxyComparer : IEqualityComparer<ProductProxy> {
        // ProductProxys are equal if their names and ProductProxy numbers are equal. 
        public bool Equals(ProductProxy x, ProductProxy y) {

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

        public int GetHashCode(ProductProxy productProxy) {
            //Check whether the object is null 
            if (object.ReferenceEquals(productProxy, null))
                return 0;

            //Calculate the hash code for the ProductProxy. 
            return (productProxy.Source == null ? 0 : productProxy.Source.GetHashCode()) ^
                (productProxy.Number == null ? 0 : productProxy.Number.GetHashCode());
        }
    }
}