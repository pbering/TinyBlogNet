using System.Threading.Tasks;

namespace TinyBlogNet.Pipeline
{
    public abstract class Processor
    {
        public virtual Task ProcessAsync(PipelineArgs args)
        {
            return Task.Run(() => { Process(args); });
        }

        public virtual void Process(PipelineArgs args)
        {
        }
    }
}