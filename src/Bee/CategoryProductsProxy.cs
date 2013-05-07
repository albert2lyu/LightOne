using System;
using System.Linq;
using Business;

namespace Bee {
    class CategoryProductsProxy : CategoryProducts {
        public override void Upsert() {
            ProxyHelper.Post<object>("CategoryProducts/Upsert", this);
        }
    }
}
