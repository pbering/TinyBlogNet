using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using Blog;
using Microsoft.Owin;
using Owin;
using TinyBlogNet;
using TinyBlogNet.IO;
using TinyBlogNet.Pipeline;

[assembly: OwinStartup(typeof(Startup))]

namespace Blog
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            const string title = "dev and ops";
            var posts = new PostRepository(new FileSystem(HostingEnvironment.MapPath("~/Posts")), new Cache());

            app.RunPipeline(new Pipeline()
                .Add(new RemoveServerHeaderProcessor())
                .Add(new RobotsProcessor())
                .Add(new RssProcessor(posts, title))
                .Add(new SitemapProcessor(posts))
                .Add(new SetLayoutProcessor(title))
                .Add(new HomeProcessor(posts))
                .Add(new PostProcessor(posts))
                .Add(new NotFoundProcessor()));
        }
    }

    internal static class AppBuilderExtensions
    {
        public static void RunPipeline(this IAppBuilder app, Pipeline pipeline)
        {
            app.Run(async ctx => { await pipeline.Run(ctx); });
        }
    }

    internal class HomeProcessor : Processor
    {
        private readonly PostRepository _posts;

        public HomeProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            if (args.Context.Request.Path.Equals(new PathString("/")))
            {
                args.Abort();
                args.Context.Response.ContentType = "text/html";

                var body = new StringBuilder();

                foreach (var post in _posts.OrderByDescending(p => p.Published))
                {
                    body.AppendFormat("<h2><a href=\"{0}\">{1}</a></h2>", post.Url, post.Title);
                    body.AppendFormat("<p>{0}</p>", post.Summary);
                    body.AppendFormat("<code>Posted {0}, tags: {1}</code>", post.Published.ToHumaneString(), string.Join(", ", post.Tags));
                }

                await args.Context.Response.WriteAsync(string.Format(args.Layout, "Home", body));
            }
        }
    }

    internal class SetLayoutProcessor : Processor
    {
        private readonly string _layout;

        public SetLayoutProcessor(string title)
        {
            _layout = "<!DOCTYPE html>\n" +
                      "<html lang=\"en-US\">" +
                      "<head>" +
                      "<meta charset=\"utf-8\">" +
                      "<meta name=\"viewport\" content=\"width=device-width,initial-scale= 1\">" +
                      "<title>{0} - Blog</title>" +
                      "<link href=\"/content/blog.css\" rel=\"stylesheet\" type=\"text/css\" media=\"all\" />" +
                      "</head>" +
                      "<body>" +
                      "<nav><a href=\"/\">" + title + "</a>&nbsp;|&nbsp;<a href=\"/rss.xml\">rss</a></nav>" +
                      "{1}" +
                      "<footer><p>reach out: <a href=\"https://twitter.com/pbering\">https://twitter.com/pbering</a></p></footer>" +
                      "</body>" +
                      "</html>";
        }

        public override void Process(PipelineArgs args)
        {
            args.Layout = _layout;
        }
    }

    internal class PostProcessor : Processor
    {
        private readonly PostRepository _posts;

        public PostProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            var path = args.Context.Request.Path;

            if (path.StartsWithSegments(new PathString("/posts")) && !path.Equals(new PathString("/posts")))
            {
                var post = _posts.FindByName(path.Value.ToLowerInvariant().Replace("/posts/", "").Trim('/'));

                if (post != null)
                {
                    args.Abort();
                    args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(1).TotalSeconds;

                    await args.Context.Response.WriteAsync(string.Format(args.Layout, post.Title, "<article>" + post.Content + "</article>"));
                }
            }
        }
    }

    internal class RobotsProcessor : Processor
    {
        public override async Task ProcessAsync(PipelineArgs args)
        {
            var path = args.Context.Request.Path;

            if (path.Value == "/robots.txt")
            {
                args.Abort();
                args.Context.Response.ContentType = "text/plain";
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(7).TotalSeconds;

                var content = new StringBuilder();

                content.Append("User-agent: *\n");
                content.Append("Disallow: /content/\n");
                content.AppendFormat("Sitemap: {0}://{1}/sitemap.xml\n", args.Context.Request.Scheme, args.Context.Request.Host.Value);

                await args.Context.Response.WriteAsync(content.ToString());
            }
        }
    }

    internal class SitemapProcessor : Processor
    {
        private readonly PostRepository _posts;

        public SitemapProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            var path = args.Context.Request.Path;

            if (path.Value == "/sitemap.xml")
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

    internal class RssProcessor : Processor
    {
        private readonly string _description;
        private readonly PostRepository _posts;
        private readonly string _title;

        public RssProcessor(PostRepository posts, string title, string description = "All blog posts")
        {
            _posts = posts;
            _title = title;
            _description = description;
        }

        public override void Process(PipelineArgs args)
        {
            var path = args.Context.Request.Path;

            if (path.Value == "/rss.xml")
            {
                args.Abort();
                args.Context.Response.ContentType = "application/rss+xml";
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(7).TotalSeconds;

                var serverUrl = args.Context.Request.Scheme + "://" + args.Context.Request.Host.Value;
                var feed = new SyndicationFeed(_title, _description, new Uri(serverUrl + "/rss.xml"), "1", DateTime.Now)
                {
                    Items = _posts.Select(
                        post => new SyndicationItem(
                            post.Title,
                            post.Summary,
                            new Uri(serverUrl + post.Url.ToString()),
                            post.Name,
                            post.Published))
                        .ToList()
                };

                var formatter = new Rss20FeedFormatter(feed);

                using (var writer = XmlWriter.Create(args.Context.Response.Body))
                {
                    formatter.WriteTo(writer);
                }
            }
        }
    }

    internal class NotFoundProcessor : Processor
    {
        public override void Process(PipelineArgs args)
        {
            args.Abort();
            args.Context.Response.StatusCode = 404;
            args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromHours(1).TotalSeconds;
        }
    }

    internal class RemoveServerHeaderProcessor : Processor
    {
        public override void Process(PipelineArgs args)
        {
            args.Context.Response.Headers.Remove("Server");
        }
    }
}