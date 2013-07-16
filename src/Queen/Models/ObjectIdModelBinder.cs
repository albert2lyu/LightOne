using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Queen.Models {
    public class ObjectIdModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
                return ObjectId.Empty;

            var attemptedValue = value.AttemptedValue;
            if (attemptedValue is string) {
                if (string.IsNullOrWhiteSpace((string)attemptedValue))
                    return ObjectId.Empty;
                return ObjectId.Parse((string)attemptedValue);
            }
            else
                return attemptedValue;
        }
    }
}