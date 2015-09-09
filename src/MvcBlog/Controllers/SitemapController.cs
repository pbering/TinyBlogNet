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
        [OutputCache(CacheProfile = "Day")]
        public async Task<ActionResult> Index()
        {
            var posts = await Task.Run(() => MvcApplication.Posts);

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var document =
                new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(ns + "urlset",
                        new XElement(ns + "url",
                            new XElement(ns + "loc", this.ServerUrl()),
                            new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                            new XElement(ns + "changefreq", "daily")),
                        from post in posts
                        select
                            new XElement(ns + "url",
                                new XElement(ns + "loc", this.ServerUrl(post.Url.ToString())),
                                new XElement(ns + "lastmod", post.Published.ToString("yyyy-MM-dd")),
                                new XElement(ns + "changefreq", "daily"))
                        )
                    );

            return Content(document.ToString(), "text/xml");
        }
    }
}