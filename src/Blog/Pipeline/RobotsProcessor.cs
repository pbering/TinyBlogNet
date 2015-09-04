using System;
using System.Text;
using System.Threading.Tasks;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class RobotsProcessor : Processor
    {
        public override async Task ProcessAsync(PipelineArgs args)
        {
            args.Abort();
            args.Context.Response.ContentType = "text/plain";
            args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromDays(30).TotalSeconds;

            var content = new StringBuilder();

            content.Append("User-agent: *\n");
            content.Append("Disallow: /content/\n");
            content.AppendFormat("Sitemap: {0}://{1}/sitemap.xml\n", args.Context.Request.Scheme, args.Context.Request.Host.Value);

            await args.Context.Response.WriteAsync(content.ToString());
        }
    }
}