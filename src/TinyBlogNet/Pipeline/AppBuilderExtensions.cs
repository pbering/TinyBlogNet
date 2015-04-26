using Owin;

namespace TinyBlogNet.Pipeline
{
    public static class AppBuilderExtensions
    {
        public static void RunPipeline(this IAppBuilder app, Pipeline pipeline)
        {
            app.Run(async ctx => { await pipeline.Run(ctx); });
        }
    }
}