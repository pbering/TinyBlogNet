using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class RemoveServerHeaderProcessor : Processor
    {
        public override void Process(PipelineArgs args)
        {
            args.Context.Response.Headers.Remove("Server");
        }
    }
}