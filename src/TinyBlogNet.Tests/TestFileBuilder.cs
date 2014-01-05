using System.IO;
using System.Text;
using TinyBlogNet.IO;

namespace TinyBlogNet.Tests
{
    public class TestFileBuilder
    {
        private readonly StringBuilder _body;
        private string _date;
        private string _endHeader;
        private string _startHeader;
        private string _summary;
        private string _tags;
        private string _title;

        public TestFileBuilder()
        {
            _body = new StringBuilder();
        }

        public TestFileBuilder Title(string title)
        {
            _title = title;

            return this;
        }

        public static TestFileBuilder Dummy()
        {
            var builder = new TestFileBuilder();

            builder.StartHeader()
                .Title("test")
                .Summary("test")
                .Date("2013-01-01")
                .Tags("test, code, spam")
                .EndHeader()
                .AddLine("test");

            return builder;
        }

        public static TestFileBuilder New()
        {
            return new TestFileBuilder();
        }

        public TestFileBuilder Summary(string summary)
        {
            _summary = summary;

            return this;
        }

        public TestFileBuilder Date(string date)
        {
            _date = date;

            return this;
        }

        public TestFileBuilder Tags(string tags)
        {
            _tags = tags;

            return this;
        }

        public TestFileBuilder StartHeader()
        {
            _startHeader = "---";

            return this;
        }

        public TestFileBuilder EndHeader()
        {
            _endHeader = "---";

            return this;
        }

        public TestFileBuilder AddLine(string line)
        {
            _body.AppendNewLine(line);

            return this;
        }

        public MarkdownFile AsMarkdownFile(string name = "this-is-a-test", string extension = "md")
        {
            return new MarkdownFile(AsFile(name, extension));
        }

        public FileBase AsFile(string name = "this-is-a-test", string extension = "md")
        {
            var content = new StringBuilder();

            if (!string.IsNullOrEmpty(_startHeader))
            {
                content.AppendNewLine(_startHeader);
            }

            if (!string.IsNullOrEmpty(_title))
            {
                content.AppendNewLine("title: " + _title);
            }

            if (!string.IsNullOrEmpty(_summary))
            {
                content.AppendNewLine("summary: " + _summary);
            }

            if (!string.IsNullOrEmpty(_date))
            {
                content.AppendNewLine("date: " + _date);
            }

            if (!string.IsNullOrEmpty(_tags))
            {
                content.AppendNewLine("tags: " + _tags);
            }

            if (!string.IsNullOrEmpty(_endHeader))
            {
                content.AppendNewLine(_endHeader);
            }

            if (!string.IsNullOrEmpty(_body.ToString()))
            {
                content.Append(_body);
            }

            var file = new TestFile(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                Name = name,
                Extension = extension
            };

            return file;
        }

        internal class TestFile : FileBase
        {
            private readonly MemoryStream _content;

            public TestFile(MemoryStream memoryStream)
            {
                _content = memoryStream;
            }

            public override Stream OpenRead()
            {
                return _content;
            }
        }
    }
}