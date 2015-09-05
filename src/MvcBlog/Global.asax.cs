using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MvcBlog
{
    public class MvcApplication : HttpApplication
    {
        public static PostRepository Posts { get; private set; }

        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            Posts = new PostRepository(new FileSystem(HostingEnvironment.MapPath("~/App_Data/Posts")), new Cache());
        }
    }
}