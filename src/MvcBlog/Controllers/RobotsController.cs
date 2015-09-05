using System;
using System.Text;
using System.Web.Mvc;

namespace MvcBlog.Controllers
{
    [RoutePrefix("robots.txt")]
    [Route("{action=Index}")]
    public class RobotsController : Controller
    {
        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public ActionResult Index()
        {
            var content = new StringBuilder();

            content.Append("User-agent: *\n");
            content.Append("Disallow: /content/\n");
            content.AppendFormat("Sitemap: {0}/sitemap.xml\n", Request.Url.GetLeftPart(UriPartial.Authority));

            return Content(content.ToString(), "text/plain");
        }
    }
}