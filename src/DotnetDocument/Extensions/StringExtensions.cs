using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetDocument.Extensions
{
    /// <summary>
    /// The string extensions class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The vowels
        /// </summary>
        private const string Vowels = "aeiouAEIOU";

        /// <summary>
        /// Substrings the between using the specified text
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="start">The start</param>
        /// <param name="end">The end</param>
        /// <returns>The string</returns>
        public static string SubstringBetween(this string text, string start, string end)
        {
            var startIndex = text.IndexOf(start, StringComparison.Ordinal);
            var endIndex = text.IndexOf(end, StringComparison.Ordinal);

            if (text.Contains(start) && text.Contains(end) && startIndex < endIndex)
                return text.Substring(startIndex + start.Length,
                    endIndex - startIndex - start.Length);

            return string.Empty;
        }

        /// <summary>
        /// Removes the start using the specified value
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="prefixToRemove">The prefix to remove</param>
        /// <returns>The string</returns>
        public static string RemoveStart(this string value, string prefixToRemove) =>
            value.StartsWith(prefixToRemove)
                ? value.Substring(prefixToRemove.Length, value.Length - prefixToRemove.Length)
                : value;

        /// <summary>
        /// Removes the start using the specified value
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="prefixesToRemove">The prefixes to remove</param>
        /// <returns>The value</returns>
        public static string RemoveStart(this string value, IEnumerable<string> prefixesToRemove)
        {
            foreach (var prefixToRemove in prefixesToRemove) value = RemoveStart(value, prefixToRemove);

            return value;
        }

        /// <summary>
        /// Removes the end using the specified value
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="suffixToRemove">The suffix to remove</param>
        /// <returns>The string</returns>
        public static string RemoveEnd(this string value, string suffixToRemove) =>
            value.EndsWith(suffixToRemove)
                ? value.Substring(0, value.Length - suffixToRemove.Length)
                : value;

        /// <summary>
        /// Removes the end using the specified value
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="suffixesToRemove">The suffixes to remove</param>
        /// <returns>The value</returns>
        public static string RemoveEnd(this string value, IEnumerable<string> suffixesToRemove)
        {
            foreach (var suffixToRemove in suffixesToRemove) value = RemoveEnd(value, suffixToRemove);

            return value;
        }

        /// <summary>
        /// Describes whether is vowel
        /// </summary>
        /// <param name="letter">The letter</param>
        /// <returns>The bool</returns>
        public static bool IsVowel(this char letter) =>
            Vowels.IndexOf(letter) >= 0;

        /// <summary>
        /// Describes whether is consonant
        /// </summary>
        /// <param name="letter">The letter</param>
        /// <returns>The bool</returns>
        public static bool IsConsonant(this char letter) =>
            char.IsLetter(letter) && !IsVowel(letter);

        /// <summary>
        /// Firsts the char to upper using the specified input
        /// </summary>
        /// <param name="input">The input</param>
        /// <returns>The string</returns>
        public static string FirstCharToUpper(this string input) => input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
        };
    }
}
