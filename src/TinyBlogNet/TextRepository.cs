using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class TextRepository : IEnumerable<Text>
    {
        private readonly Cache _cache;
        private readonly FileSystem _filesystem;

        public TextRepository(FileSystem filesystem) : this(filesystem, new NullCache())
        {
        }

        public TextRepository(FileSystem filesystem, Cache cache)
        {
            _filesystem = filesystem;
            _cache = cache;
        }

        public IEnumerator<Text> GetEnumerator()
        {
            var texts = new ConcurrentBag<Text>();

            Parallel.ForEach(_filesystem.GetFiles("*.md"), file =>
            {
                Text text;

                if (!_cache.TryGet(file.FullName, out text))
                {
                    text = new Text(new MarkdownFile(file));
                    text.Parse();

                    _cache.Add(text, file.FullName, file);
                }

                texts.Add(text);
            });

            return texts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Text FindByName(string name)
        {
            return this.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}