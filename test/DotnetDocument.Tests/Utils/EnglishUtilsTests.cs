using DotnetDocument.Utils;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Utils
{
    /// <summary>
    /// The english utils tests class
    /// </summary>
    public class EnglishUtilsTests
    {
        /// <summary>
        /// Tests that should conjugate to third person singular
        /// </summary>
        /// <param name="verb">The verb</param>
        /// <param name="expectedVerb">The expected verb</param>
        [Theory(DisplayName = "Should conjugate to third person singular")]
        [InlineData("get", "gets")]
        [InlineData("set", "sets")]
        [InlineData("init", "inits")]
        [InlineData("do", "does")]
        [InlineData("go", "goes")]
        [InlineData("publish", "publishes")]
        [InlineData("require", "requires")]
        [InlineData("watch", "watches")]
        [InlineData("miss", "misses")]
        [InlineData("rush", "rushes")]
        [InlineData("mix", "mixes")]
        [InlineData("buzz", "buzzes")]
        [InlineData("try", "tries")]
        [InlineData("carry", "carries")]
        public void ShouldConjugateToThirdPersonSingular(string verb, string expectedVerb)
        {
            // Act
            var conjugatedVerb = EnglishUtils.ConjugateToThirdPersonSingular(verb);

            // Assert
            conjugatedVerb.ShouldBe(expectedVerb);
        }
    }
}
