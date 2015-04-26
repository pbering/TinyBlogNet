using System.Collections;
using System.Collections.Generic;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public abstract class RepositoryBase<T> : IEnumerable<T> where T : class
    {
        private readonly Cache _cache;
        private readonly FileSystem _filesystem;

        protected RepositoryBase(FileSystem filesystem) : this(filesystem, new NullCache())
        {
        }

        protected RepositoryBase(FileSystem filesystem, Cache cache)
        {
            _filesystem = filesystem;
            _cache = cache;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var data = new List<T>();

            foreach (var file in _filesystem.GetFiles("*.md"))
            {
                T content;

                if (!_cache.TryGet(file.FullName, out content))
                {
                    content = Load(new MarkdownFile(file));

                    _cache.Add(content, file.FullName, file);
                }

                data.Add(content);
            }

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract T Load(MarkdownFile file);
    }
}