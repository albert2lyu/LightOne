using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Business;
using Common.Logging;
using Api.Models;

namespace Api {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(MvcApplication));
        
        protected void Application_Start() {
            Logger.Info("站点启动");
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(IEnumerable<Category>), new JsonWrapModelBinder());
            ModelBinders.Binders.Add(typeof(CategoryProducts), new JsonWrapModelBinder());

            ControllerBuilder.Current.SetControllerFactory(typeof(DummyTempDataProviderControllerFactory));
        }

        protected void Application_BeginRequest() {
            var app = HttpContext.Current;

            // 处理经过压缩的请求（部分api请求需要）
            if ("gzip" == app.Request.Headers["Content-Encoding"]) {
                app.Request.Filter = new GZipStream(app.Request.Filter, CompressionMode.Decompress);
            }

#if (DEBUG)
            //MiniProfiler.Start();
#endif
        }

        protected void Application_EndRequest() {
#if (DEBUG)
            //MiniProfiler.Stop();
#endif
        }

        protected void Application_End() {
            Logger.Info("站点停止");
        }

        private static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}"
            );
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new UnhandledExceptionFilter());
        }
    }
}