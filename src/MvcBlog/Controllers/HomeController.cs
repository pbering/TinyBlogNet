using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcBlog.Models;

namespace MvcBlog.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Home";

            var model = await Task.Run(() => new HomeModel(MvcApplication.Posts.OrderByDescending(p => p.Published)));

            return View("~/Views/Home/Index.cshtml", model);
        }
    }
}