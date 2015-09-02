using System.Web.Hosting;
using Blog;
using Blog.Pipeline;
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

            app.UsePipeline("/post", new DefaultPipeline()
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
}