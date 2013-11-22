using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostRepository _postRepository;

        public HomeController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var physicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));

            _postRepository = new PostRepository(new FileSystem(physicalPath), new Cache());
        }

        [OutputCache(CacheProfile = "DefaultLight")]
        public ActionResult Index()
        {
            var model = _postRepository.OrderByDescending(p => p.Published);

            return View("~/Views/Home/Index.cshtml", model);
        }
    }
}