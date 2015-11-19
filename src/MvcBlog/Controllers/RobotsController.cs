using System.Text;
using System.Web.Mvc;

namespace MvcBlog.Controllers
{
    [RoutePrefix("robots.txt")]
    [Route("{action=Index}")]
    public class RobotsController : Controller
    {
        [OutputCache(CacheProfile = "Month")]
        public ActionResult Index()
        {
            var content = new StringBuilder();

            content.Append("User-agent: *\n");
            content.AppendFormat("Sitemap: {0}\n", this.ServerUrl("/sitemap.xml"));

            return Content(content.ToString(), "text/plain");
        }
    }
}