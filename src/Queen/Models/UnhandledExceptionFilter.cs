using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Common.Logging;

namespace Queen.Models {
    public class UnhandledExceptionFilter : IExceptionFilter {
        private readonly ILog _Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public void OnException(ExceptionContext filterContext) {
            var routeData = filterContext.RouteData.Values;
            _Logger.Error(string.Format("controller：{0}，action：{1}，发生未捕获异常", routeData["controller"], routeData["action"]), filterContext.Exception);
        }
    }
}