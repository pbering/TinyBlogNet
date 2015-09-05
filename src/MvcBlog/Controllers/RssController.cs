using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace MvcBlog.Controllers
{
    [RoutePrefix("rss.xml")]
    [Route("{action=Index}")]
    public class RssController : Controller
    {
        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {
            var posts = await Task.Run(() => MvcApplication.Posts);
            var serverUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var feed = new SyndicationFeed("invokecommand.net", "All blog posts", new Uri(serverUrl + "/rss.xml"), "1", DateTime.Now)
            {
                Items = posts.Select(
                    post => new SyndicationItem(
                        post.Title,
                        post.Summary,
                        new Uri(serverUrl + post.Url.ToString()),
                        post.Name,
                        post.Published))
                    .ToList()
            };

            var formatter = new Rss20FeedFormatter(feed);
            var content = new StringBuilder();

            using (var writer = XmlWriter.Create(content))
            {
                formatter.WriteTo(writer);

                writer.Flush();
            }

            return Content(content.ToString(), "text/xml");
        }
    }
}