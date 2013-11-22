using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class NullCache : Cache
    {
        public override bool TryGet<T>(string key, out T @object)
        {
            @object = null;

            return false;
        }

        public override void Add(object @object, string key, FileBase file)
        {
        }
    }
}