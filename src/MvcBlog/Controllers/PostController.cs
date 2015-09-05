using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcBlog.Controllers
{
    [RoutePrefix("posts")]
    public class PostController : Controller
    {
        [OutputCache(Duration = 86400, VaryByParam = "None")]
        [Route("{*name}")]
        public async Task<ActionResult> Index(string name)
        {
            var post = await Task.Run(() => MvcApplication.Posts.FindByName(name));

            if (post != null)
            {
                ViewBag.Title = post.Title;

                return View("~/Views/Post/Index.cshtml", post);
            }

            return HttpNotFound();
        }
    }
}