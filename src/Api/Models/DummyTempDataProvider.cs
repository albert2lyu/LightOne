using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Api.Models {
    public class DummyTempDataProvider : ITempDataProvider {
        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext) {
            return null;
        }

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values) {
            
        }
    }
}