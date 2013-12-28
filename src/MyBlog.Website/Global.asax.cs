using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyBlog.Website.App_Start;

namespace MyBlog.Website
{
    public class MvcApplication : HttpApplication
    {
        private static readonly string _appName;

        static MvcApplication()
        {
            _appName = ConfigurationManager.AppSettings["MyBlog:Name"] ?? string.Empty;
        }

        public static string AppName
        {
            get { return _appName; }
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Boot();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
        }
    }
}