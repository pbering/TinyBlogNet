namespace Blog.Pipeline
{
    internal class DefaultPipeline : TinyBlogNet.Pipeline.Pipeline
    {
        public DefaultPipeline()
        {
            Add(new RemoveServerHeaderProcessor());
        }
    }
}