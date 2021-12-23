using System.Linq;
using DotnetDocument.Extensions;

namespace DotnetDocument.Utils
{
    /// <summary>
    /// The english utils class
    /// </summary>
    public static class EnglishUtils
    {
        /// <summary>
        /// Conjugates the to third person singular using the specified verb
        /// </summary>
        /// <param name="verb">The verb</param>
        /// <returns>The string</returns>
        public static string ConjugateToThirdPersonSingular(string verb)
        {
            // Check if verb ends with one of the following chars
            if (verb.EndsWith("ch") || verb.EndsWith("s") || verb.EndsWith("sh") ||
                verb.EndsWith("x") || verb.EndsWith("z") || verb.EndsWith("o"))
            {
                return $"{verb}es";
            }

            // Check if verb is at least 3 chars long, ends with a consonant then y 
            if (verb.Length > 2 && verb.Last() == 'y' && verb[^2].IsConsonant()) return $"{verb.RemoveEnd("y")}ies";

            // TODO: Handle irregular verbs

            return $"{verb}s";
        }
    }
}
