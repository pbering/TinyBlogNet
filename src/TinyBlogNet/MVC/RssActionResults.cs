using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace TinyBlogNet.MVC
{
    public class RssActionResult : ActionResult
    {
        private readonly SyndicationFeed _feed;

        public RssActionResult(SyndicationFeed feed)
        {
            _feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";

            var formatter = new Rss20FeedFormatter(_feed);

            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}