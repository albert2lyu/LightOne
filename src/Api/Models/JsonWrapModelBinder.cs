using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Api.Models
{
    public class JsonWrapModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            using (var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream)) {
                var content = reader.ReadToEnd();
                return JsonConvert.DeserializeObject(content, bindingContext.ModelType);
            }
        }
    }
}