using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetDocument.Extensions
{
    public static class StringExtensions
    {
        private const string Vowels = "aeiouAEIOU";

        public static string RemoveStart(this string value, string prefixToRemove) =>
            value.StartsWith(prefixToRemove)
                ? value.Substring(prefixToRemove.Length, value.Length - prefixToRemove.Length)
                : value;

        public static string RemoveStart(this string value, IEnumerable<string> prefixesToRemove)
        {
            foreach (var prefixToRemove in prefixesToRemove)
            {
                value = RemoveStart(value, prefixToRemove);
            }

            return value;
        }

        public static string RemoveEnd(this string value, string suffixToRemove) =>
            value.EndsWith(suffixToRemove)
                ? value.Substring(0, value.Length - suffixToRemove.Length)
                : value;

        public static string RemoveEnd(this string value, IEnumerable<string> suffixesToRemove)
        {
            foreach (var suffixToRemove in suffixesToRemove)
            {
                value = RemoveEnd(value, suffixToRemove);
            }

            return value;
        }

        public static bool IsVowel(this char letter) =>
            Vowels.IndexOf(letter) >= 0;

        public static bool IsConsonant(this char letter) =>
            char.IsLetter(letter) && !IsVowel(letter);

        public static string FirstCharToUpper(this string input) => input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
        };
    }
}
