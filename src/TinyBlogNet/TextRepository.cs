using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
            var watch = Stopwatch.StartNew();
            var texts = new ConcurrentBag<Text>();

            Parallel.ForEach(_filesystem.GetFiles("*.md"), file =>
            {
                Text text;

                if (_cache.TryGet(file.FullName, out text))
                {
                    Debug.WriteLine("Text '{0}' loaded from cache", new object[] {text.Name});
                }
                else
                {
                    text = new Text(new MarkdownFile(file));

                    text.Parse();

                    _cache.Add(text, file.FullName, file);

                    Debug.WriteLine("Text '{0}' loaded from disk", new object[] {text.Name});
                }

                texts.Add(text);
            });

            Debug.WriteLine("Texts loaded and parsed in {0} ms", watch.Elapsed.TotalMilliseconds);

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