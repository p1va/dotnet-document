using System;
using System.Linq;
using DotnetDocument.Extensions;
using Humanizer;

namespace DotnetDocument.Utils
{
    /// <summary>
    /// The format utils class
    /// </summary>
    public static class FormatUtils
    {
        /// <summary>
        /// Removes the single chars in phrase using the specified text
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>The string</returns>
        public static string RemoveSingleCharsInPhrase(string text) =>
            string.Join(" ", text
                .Split(" ")
                .Where(t => t.Length > 1));

        /// <summary>
        /// Humanizes the returns type using the specified returns type
        /// </summary>
        /// <param name="returnsType">The returns type</param>
        /// <returns>The description</returns>
        public static string HumanizeReturnsType(string returnsType)
        {
            // Nothing to return
            if (returnsType is "Task" || returnsType is "Void") return string.Empty;

            var description = string.Empty;

            // Trim returns type just in case
            returnsType = returnsType.Trim();

            // If the return starts with task
            if (returnsType.StartsWith("Task<") && returnsType.EndsWith('>'))
            {
                // This is a task
                description = "a task containing ";

                // Keep just the task content
                returnsType = returnsType
                    .RemoveStart("Task<")
                    .RemoveEnd(">");
            }

            if (returnsType.Contains("[]"))
                // It is an array
                returnsType = returnsType.Replace("[]", " array ");

            if (returnsType.Contains("<") && returnsType.Contains(">"))
            {
                // Retrieve the generic type
                var keywords = returnsType
                    .Replace(",", " and ")
                    .Split(new[]
                    {
                        '<',
                        '>'
                    }, StringSplitOptions.TrimEntries)
                    .Select(k => k.Humanize());

                var genericType = RemoveSingleCharsInPhrase(keywords.FirstOrDefault() ?? string.Empty);

                var prefix = "a";

                if (genericType.FirstOrDefault().IsVowel() is true) prefix = "an";

                // This is a generic type
                description += $" {prefix} {genericType} of {string.Join(" ", keywords.Skip(1))}";
            }
            else
            {
                var humanizeReturnsType = RemoveSingleCharsInPhrase(returnsType.Humanize());

                var prefix = "the";

                // if (humanizeReturnsType.FirstOrDefault().IsVowel() is true)
                // {
                //     prefix = "an";
                // }

                // It is a simple object
                description += $" {prefix} {humanizeReturnsType}";
            }

            // Remove single chars between white spaces
            description = description
                .Humanize()
                .FirstCharToUpper();

            return description;
        }
    }
}
