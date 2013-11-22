using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NSubstitute;
using TinyBlogNet.IO;
using Xunit;

namespace TinyBlogNet.Tests
{
    public class PostRepositoryTest
    {
        [Fact]
        public void load_posts()
        {
            //// Arrange
            var filesystem = Substitute.For<FileSystem>(new object[] {string.Empty});

            filesystem.GetFiles(Arg.Any<string>())
                .Returns(info => new[] {GetFileMock(), GetFileMock(), GetFileMock()});

            var repository = new PostRepository(filesystem);

            //// Act
            var posts = repository;

            //// Assert
            posts.Should().HaveCount(3);
        }

        [Fact]
        public void load_posts_by_tag()
        {
            //// Arrange
            var filesystem = Substitute.For<FileSystem>(new object[] {string.Empty});

            filesystem.GetFiles(Arg.Any<string>())
                .Returns(info => new[] {GetFileMock("Code, sitecore"), GetFileMock("Sitecore, Test"), GetFileMock("Test, Code")});

            var repository = new PostRepository(filesystem);

            //// Act
            var posts = repository.FindByTag("sitecore");

            //// Assert
            posts.Should().HaveCount(2);
        }

        [Fact]
        public void load_single_post_by_name()
        {
            //// Arrange
            var filesystem = Substitute.For<FileSystem>(new object[] {string.Empty});

            filesystem.GetFiles(Arg.Any<string>())
                .Returns(info => new[] {GetFileMock()});

            var repository = new PostRepository(filesystem);

            //// Act
            var post = repository.FindByName("this-is-a-test");

            //// Assert
            post.Should().NotBeNull();
            post.Name.Should().Be("this-is-a-test");
        }

        private FileBase GetFileMock(string tags = "")
        {
            var content = new StringBuilder();

            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("title: This is a test{0}", Environment.NewLine);
            content.AppendFormat("summary: This is a summary{0}", Environment.NewLine);
            content.AppendFormat("date: 2013-01-01{0}", Environment.NewLine);

            if (!string.IsNullOrEmpty(tags))
            {
                content.AppendFormat("tags: {0}{1}", tags, Environment.NewLine);
            }

            content.AppendFormat("---{0}", Environment.NewLine);
            content.AppendFormat("## TEST ##{0}", Environment.NewLine);
            content.AppendFormat("This is a Test{0}", Environment.NewLine);

            var file = Substitute.For<FileBase>();

            file.Name.Returns("this-is-a-test");
            file.Extension.Returns("md");
            file.OpenRead().Returns(new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())));

            return file;
        }
    }
}