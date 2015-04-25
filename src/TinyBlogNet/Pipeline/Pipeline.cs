using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace TinyBlogNet.Pipeline
{
    public class Pipeline
    {
        private readonly List<Processor> _processors = new List<Processor>();

        public Pipeline Add(Processor processor)
        {
            _processors.Add(processor);

            return this;
        }

        public async Task Run(IOwinContext ctx)
        {
            var args = new PipelineArgs(ctx);

            foreach (var processor in _processors)
            {
                if (!args.IsAborted)
                {
                    await processor.ProcessAsync(args);
                }
            }
        }
    }
}