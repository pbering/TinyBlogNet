using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MvcBlog.Controllers
{
    [RoutePrefix("sitemap.xml")]
    [Route("{action=Index}")]
    public class SitemapController : Controller
    {
        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {
            var posts = await Task.Run(() => MvcApplication.Posts);
            var serverUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var document =
                new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(ns + "urlset",
                        new XElement(ns + "url",
                            new XElement(ns + "loc", serverUrl),
                            new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                            new XElement(ns + "changefreq", "daily")),
                        from post in posts
                        select
                            new XElement(ns + "url",
                                new XElement(ns + "loc", serverUrl + post.Url),
                                new XElement(ns + "lastmod", post.Published.ToString("yyyy-MM-dd")),
                                new XElement(ns + "changefreq", "daily"))
                        )
                    );

            return Content(document.ToString(), "text/xml");
        }
    }
}