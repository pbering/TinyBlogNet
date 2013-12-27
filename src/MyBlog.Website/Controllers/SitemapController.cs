using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.MVC;

namespace MyBlog.Website.Controllers
{
    public class SitemapController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TextRepository _textRepository;

        public SitemapController(PostRepository postRepository, TextRepository textRepository)
        {
            _postRepository = postRepository;
            _textRepository = textRepository;
        }

        [OutputCache(CacheProfile = "DefaultSitemap")]
        public SitemapActionResult Index()
        {
            return new SitemapActionResult(_postRepository, _textRepository);
        }
    }
}