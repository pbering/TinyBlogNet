using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class TagProcessor : Processor
    {
        private readonly PostRepository _posts;

        public TagProcessor(PostRepository posts)
        {
            _posts = posts;
        }

        public override async Task ProcessAsync(PipelineArgs args)
        {
            var name = args.Context.Request.Path.Value.Trim('/');

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var posts = _posts.FindByTag(name).OrderByDescending(p => p.Published);

            if (posts.Any())
            {
                args.Abort();
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(1).TotalSeconds;
                args.Context.Response.ContentType = "text/html";

                var title = $"Posts tagged with {name}:";
                var body = new StringBuilder();

                body.AppendFormat("<h2>{0}</h2>", title);
                body.Append(DefaultLayout.GetPostListFragment(posts));

                var content = await args.Layout.RenderAsync(title, body);

                await args.Context.Response.WriteAsync(content);
            }
        }
    }
}