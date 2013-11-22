using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class RssController : Controller
    {
        private readonly PostRepository _postRepository;

        public RssController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var physicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));

            _postRepository = new PostRepository(new FileSystem(physicalPath), new Cache());
        }

        [OutputCache(CacheProfile = "DefaultRss")]
        public RssActionResult Index()
        {
            var serverUrl = ControllerContext.RequestContext.HttpContext.Request.GetServerUrl();

            var feed = new SyndicationFeed(
                "SitecorePerf Blog",
                "This is a test feed",
                new Uri(serverUrl + "/rss.xml"),
                "1",
                DateTime.Now)
            {
                Items = _postRepository.Select(
                    post => new SyndicationItem(
                        post.Title,
                        post.Summary,
                        new Uri(serverUrl + post.Url.ToString()),
                        post.Name,
                        post.Published))
                    .ToList()
            };

            return new RssActionResult(feed);
        }
    }
}