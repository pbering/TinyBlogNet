using System.Configuration;
using System.IO;
using System.Web.Hosting;
using LightInject;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.App_Start
{
    public static class Bootstrapper
    {
        public static void Boot()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var postsPhysicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));
            var textsPhysicalPath = HostingEnvironment.MapPath(dataFolder);

            var container = new ServiceContainer();

            container.Register(factory => new PostRepository(new FileSystem(postsPhysicalPath), new Cache()), new PerContainerLifetime());
            container.Register(factory => new TextRepository(new FileSystem(textsPhysicalPath), new Cache()), new PerContainerLifetime());

            container.RegisterControllers();
            container.EnableMvc();
        }
    }
}