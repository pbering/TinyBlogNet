using System;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class NotFoundProcessor : Processor
    {
        public override void Process(PipelineArgs args)
        {
            args.Abort();
            args.Context.Response.StatusCode = 404;
            args.Context.Response.Headers["Cache-Control"] = "max-age=" + TimeSpan.FromHours(1).TotalSeconds;
        }
    }
}