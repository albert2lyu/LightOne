using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Queen.Models {
    class ObjectIdModelBinderAttribute : CustomModelBinderAttribute {
        public override IModelBinder GetBinder() {
            return new ObjectIdModelBinder();
        }
    }
}