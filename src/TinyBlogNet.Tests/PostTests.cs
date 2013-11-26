using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NSubstitute;
using TinyBlogNet.Exceptions;
using TinyBlogNet.IO;
using Xunit;

namespace TinyBlogNet.Tests
{
    public class PostTests
    {
        [Fact]
        public void tags_with_same_name_is_equal()
        {
            //// Arrange
            var tag1 = new Tag("Sitecore");
            var tag2 = new Tag("sitecore");

            //// Act
            //// Assert
            tag1.Should().Be(tag2);
        }

        [Fact]
        public void parse_valid_file_with_tags()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("title: This is a test{0}", Environment.NewLine);
            content.AppendFormat("summary: This is a summary{0}", Environment.NewLine);
            content.AppendFormat("date: 2013-01-01{0}", Environment.NewLine);
            content.AppendFormat("tags: Sitecore,Performance, testing{0}", Environment.NewLine);
            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("## TEST ##{0}", Environment.NewLine);
            content.AppendFormat("This is a Test{0}", Environment.NewLine);

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var post = new Post(new MarkdownFile(file));

            //// Act
            post.Parse();

            //// Assert
            post.Tags.Should().Contain(new Tag("Sitecore"));
            post.Tags.Should().Contain(new Tag("Performance"));
            post.Tags.Should().Contain(new Tag("Testing"));
            post.Tags.Should().NotContain(new Tag("Code"));
        }

        [Fact]
        public void parse_valid_file()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("title: This is a test{0}", Environment.NewLine);
            content.AppendFormat("summary: This is a summary{0}", Environment.NewLine);
            content.AppendFormat("date: 2013-01-01{0}", Environment.NewLine);
            content.AppendFormat("tags: Test{0}", Environment.NewLine);
            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("## TEST ##{0}", Environment.NewLine);
            content.AppendFormat("This is a Test{0}", Environment.NewLine);

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var post = new Post(new MarkdownFile(file));

            //// Act
            post.Parse();

            //// Assert
            post.Name.Should().Be("this-is-a-test");
            post.Url.Should().Be(new Uri("/posts/this-is-a-test/", UriKind.Relative));
            post.Content.Should().Be("<h2>TEST</h2>\n\n<p>This is a Test</p>\n");
            post.Title.Should().Be("This is a test");
            post.Summary.Should().Be("This is a summary");
            post.Published.Should().Be(new DateTime(2013, 1, 1, 0, 0, 0, 0));
        }

        [Fact]
        public void parse_throws_exception_on_missing_title_header()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("summary: This is a summary{0}", Environment.NewLine);
            content.AppendFormat("date: 2013-01-01{0}", Environment.NewLine);
            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("## TEST ##{0}", Environment.NewLine);
            content.AppendFormat("This is a Test{0}", Environment.NewLine);

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var post = new Post(new MarkdownFile(file));

            //// Act
            Action parse = post.Parse;

            //// Assert
            parse.ShouldThrow<HeaderNotFoundException>()
                .WithMessage("'title' was not found");
        }

        [Fact]
        public void parse_throws_exception_when_no_header_present()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendFormat("## TEST ##{0}", Environment.NewLine);
            content.AppendFormat("This is a Test{0}", Environment.NewLine);

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var post = new Post(new MarkdownFile(file));

            //// Act
            Action parse = post.Parse;

            //// Assert
            parse.ShouldThrow<ParseException>()
                .WithMessage("Invalid first line");
        }
    }
}