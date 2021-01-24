using DotnetDocument.Utils;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Utils
{
    public class EnglishUtilsTests
    {
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
