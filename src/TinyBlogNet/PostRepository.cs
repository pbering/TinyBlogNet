using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class PostRepository : IEnumerable<Post>
    {
        private readonly Cache _cache;
        private readonly FileSystem _filesystem;

        public PostRepository(FileSystem filesystem) : this(filesystem, new NullCache())
        {
        }

        public PostRepository(FileSystem filesystem, Cache cache)
        {
            _filesystem = filesystem;
            _cache = cache;
        }

        public IEnumerator<Post> GetEnumerator()
        {
            var posts = new ConcurrentBag<Post>();

            Parallel.ForEach(_filesystem.GetFiles("*.md"), file =>
            {
                Post post;

                if (!_cache.TryGet(file.FullName, out post))
                {
                    post = new Post(new MarkdownFile(file));
                    post.Parse();

                    _cache.Add(post, file.FullName, file);
                }

                posts.Add(post);
            });

            return posts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Post FindByName(string name)
        {
            return this.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Post> FindByTag(string tagName)
        {
            return this.Where(post => post.Tags.Contains(new Tag(tagName)));
        }
    }
}