using DotnetDocument.Utils;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Format
{
    /// <summary>
    /// The humanizer tests class
    /// </summary>
    public class HumanizerTests
    {
        /// <summary>
        /// Tests that should humanize returns types
        /// </summary>
        /// <param name="returnsType">The returns type</param>
        /// <param name="expectedDescription">The expected description</param>
        [Theory(DisplayName = "Should humanize returns type")]
        [InlineData("ICollection<User>", "A collection of user")]
        [InlineData("List<User>", "A list of user")]
        [InlineData("IEnumerable<User>", "An enumerable of user")]
        [InlineData("Task<IList<User>>", "A task containing a list of user")]
        [InlineData("CredentialsStore?", "The credentials store")]
        [InlineData("CredentialsStore []", "The credentials store array")]
        [InlineData("OneOf<Account, Card>", "An one of of account and card")]
        [InlineData("Action<bool, string>", "An action of bool and string")]
        public void ShouldHumanizeReturnsTypes(string returnsType, string expectedDescription)
        {
            // Act
            var result = FormatUtils.HumanizeReturnsType(returnsType);

            // Assert
            result.ShouldBe(expectedDescription);
        }
    }
}
