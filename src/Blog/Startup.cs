using System;
using System.Collections.Generic;
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
            const string title = "devandops";
            var posts = new PostRepository(new FileSystem(HostingEnvironment.MapPath("~/Posts")), new Cache());

            app.UsePipeline("/rss.xml", new DefaultPipeline()
                .Add(new RssProcessor(posts, title)));

            app.UsePipeline("/robots.txt", new DefaultPipeline()
                .Add(new RobotsProcessor()));

            app.UsePipeline("/sitemap.xml", new DefaultPipeline()
                .Add(new SitemapProcessor(posts)));

            app.UsePipeline("/posts", new DefaultPipeline()
                .Add(new UseDefaultLayoutProcessor(title))
                .Add(new PostProcessor(posts))
                .Add(new NotFoundProcessor()));

            app.UsePipeline("/tag", new DefaultPipeline()
                .Add(new UseDefaultLayoutProcessor(title))
                .Add(new TagProcessor(posts))
                .Add(new NotFoundProcessor()));

            app.UsePipeline(new DefaultPipeline()
                .Add(new UseDefaultLayoutProcessor(title))
                .Add(new HomeProcessor(posts))
                .Add(new NotFoundProcessor()));
        }
    }

    internal class DefaultPipeline : Pipeline
    {
        public DefaultPipeline()
        {
            Add(new RemoveServerHeaderProcessor());
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

                var content = await args.Layout.RenderAsync("Home", DefaultLayout.GetPostListFragment(_posts.OrderByDescending(p => p.Published)));

                await args.Context.Response.WriteAsync(content);
            }
        }
    }

    internal class UseDefaultLayoutProcessor : Processor
    {
        private readonly string _siteName;

        public UseDefaultLayoutProcessor(string siteName)
        {
            _siteName = siteName;
        }

        public override void Process(PipelineArgs args)
        {
            args.Layout = new DefaultLayout(_siteName);
        }
    }

    internal class DefaultLayout : Layout
    {
        private readonly string _template;

        public DefaultLayout(string siteName)
        {
            _template = "<!DOCTYPE html>\n" +
                        "<html lang=\"en-US\">" +
                        "<head>" +
                        "<meta charset=\"utf-8\">" +
                        "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">" +
                        "<title>{0} - " + siteName + "</title>" +
                        "<link href=\"/content/blog.css\" rel=\"stylesheet\" type=\"text/css\" media=\"all\" />" +
                        "</head>" +
                        "<body>" +
                        "<nav><a href=\"/\">" + siteName + "</a> | <a href=\"/rss.xml\">rss</a></nav>" +
                        "{1}" +
                        "<footer><p>reach out: <a href=\"https://twitter.com/pbering\">twitter.com/pbering</a>, <a href=\"https://github.com/pbering\">github.com/pbering</a></p></footer>" +
                        "</body>" +
                        "</html>";
        }

        public static string GetPostListFragment(IEnumerable<Post> posts)
        {
            var content = new StringBuilder();

            foreach (var post in posts)
            {
                content.AppendFormat("<h2><a href=\"{0}\">{1}</a></h2>", post.Url, post.Title);
                content.AppendFormat("<p>{0}</p>", post.Summary);
                content.Append(GetPostInfoFragment(post));
            }

            return content.ToString();
        }

        public static string GetPostInfoFragment(Post post)
        {
            var content = new StringBuilder();

            content.AppendFormat("<code>Posted {0}, tagged: ", post.Published.ToHumaneString());

            for (var i = 0; i < post.Tags.Count; i++)
            {
                var tag = post.Tags[i];

                content.AppendFormat("<a href=\"{0}\">{1}</a>{2}", tag.Url, tag.Name, i < post.Tags.Count - 1 ? ", " : "");
            }

            content.Append("</code>");

            return content.ToString();
        }

        public override string Template
        {
            get { return _template; }
        }
    }

    internal class TagProcessor : Processor
    {
        private readonly PostRepository _posts;

        public TagProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            var name = args.Context.Request.Path.Value.Trim('/');

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var posts = _posts.FindByTag(name).OrderByDescending(p => p.Published);

            if (posts.Any())
            {
                args.Abort();
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(1).TotalSeconds;

                var title = string.Format("Posts tagged with {0}:", name);
                var body = new StringBuilder();

                body.AppendFormat("<h2>{0}</h2>", title);
                body.Append(DefaultLayout.GetPostListFragment(posts));

                var content = await args.Layout.RenderAsync(title, body);

                await args.Context.Response.WriteAsync(content);
            }
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
            var name = args.Context.Request.Path.Value.Trim('/');

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var post = _posts.FindByName(name);

            if (post != null)
            {
                args.Abort();
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(1).TotalSeconds;

                var body = new StringBuilder();

                body.AppendFormat("<article>{0}</article>", post.Content);
                body.Append(DefaultLayout.GetPostInfoFragment(post));

                var content = await args.Layout.RenderAsync(post.Title, body);

                await args.Context.Response.WriteAsync(content);
            }
        }
    }

    internal class RobotsProcessor : Processor
    {
        public override async Task ProcessAsync(PipelineArgs args)
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