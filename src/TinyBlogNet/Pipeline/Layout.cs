using System.Threading.Tasks;

namespace TinyBlogNet.Pipeline
{
    public abstract class Layout
    {
        public abstract string Template { get; }

        public async Task<string> RenderAsync(params object[] args)
        {
            return await Task.Run(() => string.Format(Template, args));
        }
    }
}