using System.Linq;
using System.Web.Mvc;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostRepository _postRepository;

        public HomeController(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [OutputCache(CacheProfile = "DefaultLight")]
        public ActionResult Index()
        {
            var model = _postRepository.OrderByDescending(p => p.Published);

            return View("~/Views/Home/Index.cshtml", model);
        }
    }
}