using System;
using FluentAssertions;
using TinyBlogNet.Exceptions;
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
            var file = TestFileBuilder.Dummy()
                .Tags("Sitecore,Performance, testing")
                .AsMarkdownFile();

            var post = new Post(file);

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
            var file = TestFileBuilder.New()
                .StartHeader()
                .Title("This is a test")
                .Summary("This is a summary")
                .Date("2013-01-01")
                .EndHeader()
                .AddLine("## TEST ##")
                .AddLine("This is a Test")
                .AsMarkdownFile("this-is-another-test");

            var post = new Post(file);

            //// Act
            post.Parse();

            //// Assert
            post.Name.Should().Be("this-is-another-test");
            post.Url.Should().Be(new Uri("/posts/this-is-another-test/", UriKind.Relative));
            post.Content.Should().Be("<h2>TEST</h2>\n\n<p>This is a Test</p>\n");
            post.Title.Should().Be("This is a test");
            post.Summary.Should().Be("This is a summary");
            post.Published.Should().Be(new DateTime(2013, 1, 1, 0, 0, 0, 0));
        }

        [Fact]
        public void parse_throws_exception_on_missing_title_header()
        {
            //// Arrange
            var file = TestFileBuilder.Dummy()
                .Title(null)
                .AsMarkdownFile();

            var post = new Post(file);

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
            var file = TestFileBuilder.Dummy()
                .Date("2013-13-01")
                .AsMarkdownFile();

            var post = new Post(file);

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
            var file = TestFileBuilder.New()
                .AddLine("## TEST ##")
                .AsMarkdownFile();

            var post = new Post(file);

            //// Act
            Action parse = post.Parse;

            //// Assert
            parse.ShouldThrow<ParseException>()
                .WithMessage("Invalid first line");
        }
    }
}