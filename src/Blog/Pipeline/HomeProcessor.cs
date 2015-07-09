using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class HomeProcessor : Processor
    {
        private readonly PostRepository _posts;

        public HomeProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            if (args.Context.Request.Path.Equals(new PathString("/")))
            {
                args.Abort();
                args.Context.Response.ContentType = "text/html";

                var content = await args.Layout.RenderAsync("Home", DefaultLayout.GetPostListFragment(_posts.OrderByDescending(p => p.Published)));

                await args.Context.Response.WriteAsync(content);
            }
        }
    }
}