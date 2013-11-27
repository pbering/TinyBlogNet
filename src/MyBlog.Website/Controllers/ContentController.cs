using System.Web.Mvc;
using TinyBlogNet;

namespace MyBlog.Website.Controllers
{
    public class ContentController : Controller
    {
        private readonly TextRepository _textRepository;

        public ContentController(TextRepository textRepository)
        {
            _textRepository = textRepository;
        }

        [OutputCache(CacheProfile = "DefaultLight")]
        public ActionResult Index(string name)
        {
            var model = _textRepository.FindByName(name);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Content/Index.cshtml", model);
        }
    }
}