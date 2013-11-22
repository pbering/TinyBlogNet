using System.Collections.Generic;
using System.Runtime.Caching;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class Cache
    {
        private readonly MemoryCache _cache;

        public Cache()
        {
            _cache = MemoryCache.Default;
        }

        public virtual bool TryGet<T>(string key, out T @object) where T : class
        {
            var found = _cache.Get(key) as T;

            if (found != null)
            {
                @object = found;

                return true;
            }

            @object = null;

            return false;
        }

        public virtual void Add(object @object, string key, FileBase file)
        {
            var policy = new CacheItemPolicy();

            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string>
            {
                file.FullName
            }));

            _cache.Set(key, @object, policy);
        }
    }
}