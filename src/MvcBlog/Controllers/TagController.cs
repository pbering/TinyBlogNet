using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcBlog.Models;

namespace MvcBlog.Controllers
{
    [RoutePrefix("tags")]
    public class TagController : Controller
    {
        [OutputCache(CacheProfile = "Day")]
        [Route("{*name}")]
        public async Task<ActionResult> Index(string name)
        {
            var model = await Task.Run(() => new HomeModel(MvcApplication.Posts.FindByTag(name).OrderByDescending(p => p.Published)));

            if (model.Posts.Any())
            {
                ViewBag.Title = $"Posts tagged with {name}:";

                return View("~/Views/Tag/Index.cshtml", model);
            }

            return HttpNotFound();
        }
    }
}