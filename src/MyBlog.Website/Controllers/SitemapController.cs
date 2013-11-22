using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.Controllers
{
    public class SitemapController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TextRepository _textRepository;

        public SitemapController()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var postsPhysicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));
            var textsPhysicalPath = HostingEnvironment.MapPath(dataFolder);

            _postRepository = new PostRepository(new FileSystem(postsPhysicalPath), new Cache());
            _textRepository = new TextRepository(new FileSystem(textsPhysicalPath), new Cache());
        }

        [OutputCache(CacheProfile = "DefaultSitemap")]
        public SitemapActionResult Index()
        {
            return new SitemapActionResult(_postRepository, _textRepository);
        }
    }
}