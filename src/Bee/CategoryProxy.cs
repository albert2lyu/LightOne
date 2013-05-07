using System;
using System.Collections.Generic;
using System.Linq;
using Business;

namespace Bee
{
    class CategoryProxy : Category {
        public new static IEnumerable<Category> Upsert(IEnumerable<Category> categories) {
            return ProxyHelper.Post<IEnumerable<Category>>("Categories/Upsert", categories);
        }
    }
}
