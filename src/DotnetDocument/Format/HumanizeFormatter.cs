using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Extensions;
using DotnetDocument.Utils;
using Humanizer;

namespace DotnetDocument.Format
{
    public class HumanizeFormatter : IFormatter
    {
        private readonly DocumentationOptions options;

        public HumanizeFormatter(DocumentationOptions options) =>
            (this.options) = (options);

        public string FormatName(string template, params (string key, string value)[] names)
        {
            var formattedName = template;

            // Remove suffixes
            foreach (var name in names)
            {
                // Humanize the name to split word on case change
                var humanizedName = name.value
                    .RemoveStart(options.PrefixesToRemove)
                    .RemoveEnd(options.SuffixesToRemove)
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

        public string ConjugateThirdPersonSingular(string verb)
        {
            // Check if there is a custom verb conjugation
            if (options.Verbs.ContainsKey(verb.ToLowerInvariant()))
            {
                return options.Verbs[verb];
            }

            return EnglishUtils.ConjugateToThirdPersonSingular(verb);
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
                .RemoveEnd(options.SuffixesToRemove);

            // Humanize the name to split word on case change
            var humanizedMethodName = cleanAction.Humanize().ToLower();

            // Identify the words that compose the method name
            var words = humanizedMethodName.Split(" ");

            // Take the first word and format it as a verb
            var verb = ConjugateThirdPersonSingular(words.First());

            // Check if method is test
            var isTestMethod = attributes.Any(a => options.TestAttributes.Contains(a));

            // Check if method is static
            var isStaticMethod = modifiers.Any(m => m == "static");

            switch (returns: returnType, @static: isStaticMethod, test: isTestMethod, wordsCount: words.Length,
                parametersCount: humanizedParameters.Count())
            {
                // Method marked with tests attributes
                case { test: true }:
                    return options.Method.Summary.TestMethod
                        .Replace(TemplateKeys.Verb, humanizedMethodName)
                        .FirstCharToUpper();

                // Instance boolean method
                case { returns: "bool", @static: false }:
                    return options.Method.Summary.Instance.BoolMethod
                        .Replace(TemplateKeys.Verb, humanizedMethodName)
                        .FirstCharToUpper();

                // Static boolean method
                case { returns: "bool", @static: true }:
                    return options.Method.Summary.Static.BoolMethod
                        .Replace(TemplateKeys.Verb, humanizedMethodName)
                        .FirstCharToUpper();

                // One word, 0 params static method
                case { wordsCount: 1, parametersCount: 0, @static: true }:
                    return options.Method.Summary.Static.ZeroArgsOneWordMethod
                        .Replace(TemplateKeys.Verb, verb)
                        .FirstCharToUpper();

                // One word, 0 params instance method
                case { wordsCount: 1, parametersCount: 0, @static: false }:
                    return options.Method.Summary.Instance.ZeroArgsOneWordMethod
                        .Replace(TemplateKeys.Verb, verb)
                        .FirstCharToUpper();

                // One word method with params
                case { wordsCount: 1, parametersCount: > 0 }:
                    return options.Method.Summary.ManyArgsOneWordMethod
                        .Replace(TemplateKeys.Verb, verb)
                        .Replace(TemplateKeys.FirstParam, humanizedParameters.First())
                        .FirstCharToUpper();

                // Multiple words method with params
                case { wordsCount: > 1, parametersCount: > 0 }:
                    return options.Method.Summary.ManyArgsManyWordMethod
                        .Replace(TemplateKeys.Verb, verb)
                        .Replace(TemplateKeys.Object, string.Join(" ", words.Skip(1)))
                        .Replace(TemplateKeys.FirstParam, humanizedParameters.First())
                        .FirstCharToUpper();

                default:
                    return options.Method.Summary.Default
                        .Replace(TemplateKeys.Verb, verb)
                        .Replace(TemplateKeys.Object, string.Join(" ", words.Skip(1)))
                        .FirstCharToUpper();
            }
        }
    }
}
