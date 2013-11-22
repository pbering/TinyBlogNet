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
            var watch = Stopwatch.StartNew();
            var posts = new ConcurrentBag<Post>();

            Parallel.ForEach(_filesystem.GetFiles("*.md"), file =>
            {
                Post post;

                if (_cache.TryGet(file.FullName, out post))
                {
                    Debug.WriteLine("Post '{0}' loaded from cache", new object[] {post.Title});
                }
                else
                {
                    post = new Post(new MarkdownFile(file));

                    post.Parse();

                    _cache.Add(post, file.FullName, file);

                    Debug.WriteLine("Post '{0}' loaded from disk", new object[] {post.Title});
                }

                posts.Add(post);
            });

            Debug.WriteLine("Posts loaded and parsed in {0} ms", watch.Elapsed.TotalMilliseconds);

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