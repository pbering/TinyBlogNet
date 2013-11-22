using System;
using FluentAssertions;
using Xunit;

namespace TinyBlogNet.Tests
{
    public class HumaneDateStringTests
    {
        [Fact]
        public void one_day_should_return_yesterday()
        {
            //// Arrange
            var source = new DateTime(2013, 1, 1);
            var now = new DateTime(2013, 1, 2);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("yesterday");
        }

        [Fact]
        public void two_days_should_return_two_days_ago()
        {
            //// Arrange
            var source = new DateTime(2013, 1, 1);
            var now = new DateTime(2013, 1, 3);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("2 days ago");
        }

        [Fact]
        public void two_years_should_return_two_years_ago()
        {
            //// Arrange
            var source = new DateTime(2013, 1, 1);
            var now = new DateTime(2015, 2, 1);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("2 years ago");
        }

        [Fact]
        public void one_month_should_return_one_month_ago()
        {
            //// Arrange
            var source = new DateTime(2013, 1, 1);
            var now = new DateTime(2013, 2, 1);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("one month ago");
        }

        [Fact]
        public void seven_days_should_return_seven_days_ago()
        {
            //// Arrange
            var source = new DateTime(2013, 11, 17);
            var now = new DateTime(2013, 11, 24);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("7 days ago");
        }

        [Fact]
        public void same_day_should_return_today()
        {
            //// Arrange
            var source = new DateTime(2013, 1, 1);
            var now = new DateTime(2013, 1, 1);

            //// Act
            var text = source.ToHumaneString(now);

            //// Assert
            text.Should().Be("today");
        }
    }
}