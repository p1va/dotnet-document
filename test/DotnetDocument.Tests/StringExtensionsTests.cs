using DotnetDocument.Extensions;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("Initializes a new instance of the <<GenericRepository{T}>> class.", "GenericRepository{T}")]
        [InlineData("Initializes a new instance of the <<UserRepository>> class.", "UserRepository")]
        public void ShouldSubstring(string text, string expected)
        {
            // Act
            var substring = text.SubstringBetween("<<", ">>");

            // Assert
            substring.ShouldBe(expected);
        }

        [Theory]
        [InlineData("Initializes a new instance of the UserRepository class.", "")]
        public void ShouldNotSubstring(string text, string expected)
        {
            // Act
            var substring = text.SubstringBetween("<<", ">>");

            // Assert
            substring.ShouldBe(expected);
        }
    }
}
