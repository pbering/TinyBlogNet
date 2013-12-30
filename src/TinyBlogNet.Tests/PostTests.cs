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

            content.AppendNewLine("---");
            content.AppendNewLine("title: This is a test");
            content.AppendNewLine("summary: This is a summary");
            content.AppendNewLine("date: 2013-01-01");
            content.AppendNewLine("tags: Sitecore,Performance, testing");
            content.AppendNewLine("---");
            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

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

            content.AppendNewLine("---");
            content.AppendNewLine("title: This is a test");
            content.AppendNewLine("summary: This is a summary");
            content.AppendNewLine("date: 2013-01-01");
            content.AppendNewLine("tags: Test");
            content.AppendNewLine("---");
            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

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

            content.AppendNewLine("---");
            content.AppendNewLine("summary: This is a summary");
            content.AppendNewLine("date: 2013-01-01");
            content.AppendNewLine("---");
            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

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
        public void parse_throws_exception_on_invalid_date()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendNewLine("---");
            content.AppendNewLine("title: Title");
            content.AppendNewLine("summary: This is a summary");
            content.AppendNewLine("date: 2013-13-01");
            content.AppendNewLine("---");
            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var post = new Post(new MarkdownFile(file));

            //// Act
            Action parse = post.Parse;

            //// Assert
            parse.ShouldThrow<InvalidHeaderValueException>()
                .WithMessage("The header 'date' with value '2013-13-01' could not be parsed as DateTime");
        }

        [Fact]
        public void parse_throws_exception_when_no_header_present()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

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