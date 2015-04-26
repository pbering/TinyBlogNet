using System;
using System.Linq;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class TextRepository : RepositoryBase<Text>
    {
        public TextRepository(FileSystem filesystem) : base(filesystem)
        {
        }

        public TextRepository(FileSystem filesystem, Cache cache) : base(filesystem, cache)
        {
        }

        public Text FindByName(string name)
        {
            return this.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        protected override Text Load(MarkdownFile file)
        {
            var text = new Text(file);

            text.Parse();

            return text;
        }
    }
}