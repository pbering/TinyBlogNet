using System.Linq;
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
            var posts = repository.FindByTag("sitecore").ToList();

            //// Assert
            posts.Should().HaveCount(2);

            foreach (var post in posts)
            {
                post.Tags.Count.Should().Be(2);
                post.Tags.Should().Contain(new Tag("Sitecore"));
            }
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
            return TestFileBuilder.Dummy().Tags(tags).AsFile();
        }
    }
}