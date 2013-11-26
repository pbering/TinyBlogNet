using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostRepository _postRepository;

        public PostsController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var physicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));

            _postRepository = new PostRepository(new FileSystem(physicalPath), new Cache());
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