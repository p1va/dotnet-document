using DotnetDocument.Extensions;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests
{
    /// <summary>
    /// The string extensions tests class
    /// </summary>
    public class StringExtensionsTests
    {
        /// <summary>
        /// Tests that should substring
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="expected">The expected</param>
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

        /// <summary>
        /// Tests that should not substring
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="expected">The expected</param>
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
