using System;
using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Extensions;
using Humanizer;

namespace DotnetDocument.Format
{
    public class HumanizeFormatter : IFormatter
    {
        // TODO: Read from config
        private static readonly string[] _prefixesToRemove =
        {
            "I",
        };

        // TODO: Read from config
        private static readonly string[] _prefixToRemove =
        {
            "_"
        };

        // TODO: Read from config
        private static readonly string[] _suffixToRemove =
        {
            "Class",
            "Async"
        };

        // TODO: Read from config
        private static readonly string[] _testMethodsAttributes =
        {
            "Theory",
            "Fact",
            "TestMethod",
            "Test",
            "TestCase",
            "DataTestMethod"
        };

        // TODO: Read from config
        private static readonly Dictionary<string, string> _verbsAliases = new()
        {
            {
                "to", "returns"
            },
            {
                "from", "creates"
            },
            {
                "try", "tries"
            },
            {
                "as", "converts"
            },
            {
                "with", "adds"
            },
            {
                "setup", "setup"
            },
        };

        // TODO: Read from config
        private static readonly Dictionary<string, string> _nameAliases = new()
        {
            {
                "sut", "system under test"
            }
        };

        public string FormatName(string template, params (string key, string value)[] names)
        {
            var formattedName = template;

            // Remove suffixes
            foreach (var name in names)
            {
                // Humanize the name to split word on case change
                var humanizedName = name.value
                    .RemoveStart(_prefixToRemove)
                    .RemoveEnd(_suffixToRemove)
                    .Humanize()
                    .ToLower();

                // Remove single chars between white spaces
                humanizedName = string.Join(" ", humanizedName
                    .Split(" ")
                    .Where(t => t.Length > 1));

                // Replace placeholder
                formattedName = formattedName.Replace(name.key, humanizedName);
            }

            // Humanize once again to only have the first letter as upper case
            return formattedName.FirstCharToUpper();
        }

        public string FormatInherits(string template, string key, params string[] names)
        {
            var inheritsFromDescription = string.Join(" and ", names);

            var inherits = template.Replace(key, inheritsFromDescription);

            return inherits;
        }

        public string FormatVerb(string verb)
        {
            if (_verbsAliases.ContainsKey(verb.ToLowerInvariant()))
            {
                return _verbsAliases[verb];
            }

            return $"{verb}s";
        }

        public string FormatMethod(string name, string returnType, IEnumerable<string> modifiers,
            IEnumerable<string> parameters, IEnumerable<string> attributes)
        {
            // Humanize parameter names
            var humanizedParameters = parameters
                .Select(p => p.Humanize().ToLower())
                .ToList();

            // Remove suffixes
            var cleanAction = name
                .Replace("_", " ")
                .RemoveEnd(_suffixToRemove);

            // Humanize the name to split word on case change
            var humanizedMethodName = cleanAction.Humanize().ToLower();

            // Identify the words that compose the method name
            var words = humanizedMethodName.Split(" ");

            // Take the first word and format it as a verb
            var verb = FormatVerb(words.First());

            // Check if method is test
            var isTestMethod = attributes.Any(a => _testMethodsAttributes.Contains(a));

            // Check if method is static
            var isStaticMethod = modifiers.Any(m => m == "static");

            switch (returns: returnType, @static: isStaticMethod, test: isTestMethod, wordsCount: words.Length,
                parametersCount: humanizedParameters.Count())
            {
                // Method marked with tests attributes
                case {test: true}:
                    return "tests that {{verb}}"
                        .Replace("{{verb}}", humanizedMethodName)
                        .FirstCharToUpper();

                // Instance boolean method
                case {returns: "bool", @static: false}:
                    return "describes whether this instance {{verb}}"
                        .Replace("{{verb}}", humanizedMethodName)
                        .FirstCharToUpper();

                // Static boolean method
                case {returns: "bool", @static: true}:
                    return "describes whether {{verb}}"
                        .Replace("{{verb}}", humanizedMethodName)
                        .FirstCharToUpper();

                // One word, 0 params static method
                case {wordsCount: 1, parametersCount: 0, @static: true}:
                    return "{{verb}}"
                        .Replace("{{verb}}", verb)
                        .FirstCharToUpper();

                // One word, 0 params instance method
                case {wordsCount: 1, parametersCount: 0, @static: false}:
                    return "{{verb}} this instance"
                        .Replace("{{verb}}", verb)
                        .FirstCharToUpper();

                // One word method with params
                case {wordsCount: 1, parametersCount: > 0}:
                    return "{{verb}} the {{parameter}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{parameter}}", humanizedParameters.First())
                        .FirstCharToUpper();

                // Multiple words method with params
                case {wordsCount: > 1, parametersCount: > 0}:
                    return "{{verb}} the {{object}} using the specified {{parameter}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{object}}", string.Join(" ", words.Skip(1)))
                        .Replace("{{parameter}}", humanizedParameters.First())
                        .FirstCharToUpper();

                default:
                    return "{{verb}} the {{object}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{object}}", string.Join(" ", words.Skip(1)))
                        .FirstCharToUpper();
            }
        }
    }
}
