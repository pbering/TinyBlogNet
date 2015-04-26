using System;
using System.Collections.Generic;
using System.Linq;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class PostRepository : RepositoryBase<Post>
    {
        public PostRepository(FileSystem filesystem) : base(filesystem)
        {
        }

        public PostRepository(FileSystem filesystem, Cache cache) : base(filesystem, cache)
        {
        }

        public Post FindByName(string name)
        {
            return this.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Post> FindByTag(string tagName)
        {
            return this.Where(post => post.Tags.Contains(new Tag(tagName)));
        }

        protected override Post Load(MarkdownFile file)
        {
            var post = new Post(file);

            post.Parse();

            return post;
        }
    }
}