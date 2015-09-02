using System;
using System.Text;
using System.Threading.Tasks;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class PostProcessor : Processor
    {
        private readonly PostRepository _posts;

        public PostProcessor(PostRepository posts)
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

            var post = _posts.FindByName(name);

            if (post != null)
            {
                args.Abort();
                args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(7).TotalSeconds;
                args.Context.Response.ContentType = "text/html";

                var body = new StringBuilder();

                body.AppendFormat("<h1><a href=\"{0}\">{1}</a></h1>", post.Url, post.Title);
                body.AppendFormat("<article>{0}</article>", post.Content);
                body.Append(DefaultLayout.GetPostInfoFragment(post));

                var content = await args.Layout.RenderAsync(post.Title, body);

                await args.Context.Response.WriteAsync(content);
            }
        }
    }
}