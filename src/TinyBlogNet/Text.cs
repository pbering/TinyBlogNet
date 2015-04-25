using System;

namespace TinyBlogNet
{
    public class Text
    {
        private readonly MarkdownFile _file;

        public Text(MarkdownFile file)
        {
            _file = file;

            Modified = _file.Modified;
            Name = _file.Name;
            Url = new Uri("/content/" + _file.Name.ToLowerInvariant(), UriKind.Relative);
        }

        public Uri Url { get; private set; }
        public string Name { get; private set; }
        public string Content { get; private set; }
        public string Title { get; private set; }
        public DateTime Modified { get; set; }

        public void Parse()
        {
            Title = _file.GetHeaderValue("title");
            Content = _file.Body;
        }
    }
}