using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class UseDefaultLayoutProcessor : Processor
    {
        private readonly string _siteName;

        public UseDefaultLayoutProcessor(string siteName)
        {
            _siteName = siteName;
        }

        public override void Process(PipelineArgs args)
        {
            args.Layout = new DefaultLayout(_siteName);
        }
    }
}