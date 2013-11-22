using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class TagsController : Controller
    {
        private readonly PostRepository _postRepository;

        public TagsController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var physicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));

            _postRepository = new PostRepository(new FileSystem(physicalPath), new Cache());
        }

        [OutputCache(CacheProfile = "DefaultLight")]
        public ActionResult Index(string name)
        {
            var model = _postRepository.FindByTag(name).OrderByDescending(p => p.Published);

            if (!model.Any())
            {
                return HttpNotFound();
            }

            return View("~/Views/Tags/Index.cshtml", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult All()
        {
            var model = new List<Tag>();

            foreach (var tag in _postRepository.SelectMany(post => post.Tags.Where(tag => !model.Contains(tag))))
            {
                model.Add(tag);
            }

            return PartialView("~/Views/Tags/All.cshtml", model.OrderBy(t => t.Name));
        }
    }
}