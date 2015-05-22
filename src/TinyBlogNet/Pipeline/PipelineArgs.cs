using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace TinyBlogNet.Pipeline
{
    public class PipelineArgs
    {
        public PipelineArgs(IOwinContext ctx)
        {
            Context = ctx;
        }

        public Layout Layout { get; set; }
        public IOwinContext Context { get; private set; }
        public bool IsAborted { get; private set; }

        public void Abort()
        {
            IsAborted = true;
        }
    }

}