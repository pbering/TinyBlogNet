using System.Web.Mvc;

namespace MyBlog.Website.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            HttpContext.Response.StatusCode = 404;
            HttpContext.Response.TrySkipIisCustomErrors = true;
            HttpContext.Response.Clear();

            return View("~/Views/Error/NotFound.cshtml");
        }
    }
}