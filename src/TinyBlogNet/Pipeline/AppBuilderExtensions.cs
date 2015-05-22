using Owin;

namespace TinyBlogNet.Pipeline
{
    public static class AppBuilderExtensions
    {
        public static void UsePipeline(this IAppBuilder app, Pipeline pipeline)
        {
            app.Run(async ctx => { await pipeline.Run(new PipelineArgs(ctx)); });
        }

        public static void UsePipeline(this IAppBuilder app, string path, Pipeline pipeline)
        {
            app.Map(path, application => { application.UsePipeline(pipeline); });
        }
    }
}