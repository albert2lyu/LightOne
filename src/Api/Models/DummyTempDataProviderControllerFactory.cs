using System;
using System.Linq;
using System.Web.Mvc;

namespace Api.Models {
    public class DummyTempDataProviderControllerFactory : DefaultControllerFactory {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType) {
            var controller = base.GetControllerInstance(requestContext, controllerType) as Controller;
            if (controller != null)
                controller.TempDataProvider = new DummyTempDataProvider();

            return controller;
        }
    }
}