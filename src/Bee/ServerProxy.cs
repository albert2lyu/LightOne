using System;
using System.Collections.Generic;
using System.Linq;
using Business;

namespace Bee {
    class ServerProxy {
        public static IEnumerable<Category> UpsertCategories(IEnumerable<Category> categories) {
            return ProxyHelper.Post<IEnumerable<Category>>("Categories/Upsert", categories);
        }
    }
}