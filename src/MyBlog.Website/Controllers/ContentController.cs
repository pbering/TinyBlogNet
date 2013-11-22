using System.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class ContentController : Controller
    {
        private readonly TextRepository _textRepository;

        public ContentController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var physicalPath = HostingEnvironment.MapPath(dataFolder);

            _textRepository = new TextRepository(new FileSystem(physicalPath), new Cache());
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