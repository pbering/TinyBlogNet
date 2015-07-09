using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class SitemapProcessor : Processor
    {
        private readonly PostRepository _posts;

        public SitemapProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            args.Abort();
            args.Context.Response.ContentType = "text/xml";
            args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(7).TotalSeconds;

            var serverUrl = args.Context.Request.Scheme + "://" + args.Context.Request.Host.Value;

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
                        )
                    );

            await args.Context.Response.WriteAsync(document.ToString());
        }
    }
}