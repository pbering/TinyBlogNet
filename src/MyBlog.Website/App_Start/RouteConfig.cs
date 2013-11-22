using System.Web.Mvc;
using System.Web.Routing;

namespace MyBlog.Website.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Sitemap", "sitemap.xml",
                new
                {
                    controller = "Sitemap",
                    action = "Index"
                });

            routes.MapRoute("Rss", "rss.xml",
                new
                {
                    controller = "Rss",
                    action = "Index"
                });

            routes.MapRoute("Direct", "{controller}/{name}",
                new
                {
                    action = "Index"
                });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                });
        }
    }
}