using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class RssController : Controller
    {
        private readonly PostRepository _postRepository;

        public RssController(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [OutputCache(CacheProfile = "DefaultRss")]
        public RssActionResult Index()
        {
            var serverUrl = ControllerContext.RequestContext.HttpContext.Request.GetServerUrl();
            var feed = new SyndicationFeed(MvcApplication.AppName, "Feed of all posts",
                new Uri(serverUrl + "/rss.xml"), "1", DateTime.Now)
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