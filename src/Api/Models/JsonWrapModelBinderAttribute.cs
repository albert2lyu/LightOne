using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Api.Models {
    class JsonWrapModelBinderAttribute : CustomModelBinderAttribute {
        public override IModelBinder GetBinder() {
            return new JsonWrapModelBinder();
        }
    }
}