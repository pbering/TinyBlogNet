using System;
using System.Text;
using System.Web.Mvc;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class RobotsController : Controller
    {
        [OutputCache(CacheProfile = "DefaultRobots")]
        public ActionResult Index()
        {
            var content = new StringBuilder();

            content.AppendFormat("user-agent: *{0}", Environment.NewLine);
            content.AppendFormat("disallow: /scripts/{0}", Environment.NewLine);
            content.AppendFormat("disallow: /fonts/{0}", Environment.NewLine);
            content.AppendFormat("disallow: /content/{0}", Environment.NewLine);
            content.AppendFormat("disallow: /views/{0}", Environment.NewLine);
            content.AppendFormat("sitemap: {0}/sitemap.xml{1}", ControllerContext.RequestContext.HttpContext.Request.GetServerUrl(), Environment.NewLine);

            return Content(content.ToString(), "text/plain");
        }
    }
}