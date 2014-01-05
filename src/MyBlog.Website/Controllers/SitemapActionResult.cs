using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class SitemapActionResult : ActionResult
    {
        private readonly IEnumerable<Post> _posts;
        private readonly IEnumerable<Text> _texts;

        public SitemapActionResult(IEnumerable<Post> posts, IEnumerable<Text> texts)
        {
            _posts = posts;
            _texts = texts;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var serverUrl = context.RequestContext.HttpContext.Request.GetServerUrl();
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var document =
                new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(ns + "urlset",
                        new XElement(ns + "url",
                            new XElement(ns + "loc", serverUrl),
                            new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                            new XElement(ns + "changefreq", "daily")),
                        from post in _posts
                        select
                            new XElement(ns + "url",
                                new XElement(ns + "loc", serverUrl + post.Url),
                                new XElement(ns + "lastmod", post.Published.ToString("yyyy-MM-dd")),
                                new XElement(ns + "changefreq", "daily"))
                        , from text in _texts
                            select
                                new XElement(ns + "url",
                                    new XElement(ns + "loc", serverUrl + text.Url),
                                    new XElement(ns + "lastmod", text.Modified.ToString("yyyy-MM-dd")),
                                    new XElement(ns + "changefreq", "monthly")))
                    );

            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            context.HttpContext.Response.Write(document.ToString());
        }
    }
}