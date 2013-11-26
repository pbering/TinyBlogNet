using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NSubstitute;
using TinyBlogNet.IO;
using Xunit;

namespace TinyBlogNet.Tests
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendNewLine(this StringBuilder builder, string value)
        {
            builder.AppendFormat("{0}{1}", value, Environment.NewLine);
            
            return builder;
        }
    }

    public class TextRepositoryTests
    {
        [Fact]
        public void can_load_file_from_name()
        {
            //// Arrange
            var content = new StringBuilder();

            content.AppendNewLine("---");
            content.AppendNewLine("title: This is a test");
            content.AppendNewLine("---");
            content.AppendNewLine("## TEST ##");
            content.AppendNewLine("This is a Test");

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            var filesystem = Substitute.For<FileSystem>(new object[] {string.Empty});

            filesystem.GetFiles(Arg.Any<string>())
                .Returns(info => new[] {file});

            var repository = new TextRepository(filesystem);

            //// Act
            var text = repository.FindByName("this-is-a-test");

            //// Assert
            text.Should().NotBeNull();
            text.Name.Should().Be("this-is-a-test");
            text.Title.Should().Be("This is a test");
            text.Url.Should().Be(new Uri("/content/this-is-a-test/", UriKind.Relative));
        }
    }
}