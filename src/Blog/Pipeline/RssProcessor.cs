using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
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
}