using System.Web.Mvc;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostRepository _postRepository;

        public PostsController(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [OutputCache(CacheProfile = "DefaultLight")]
        public ActionResult Index(string name)
        {
            var model = _postRepository.FindByName(name);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Posts/Index.cshtml", model);
        }
    }
}