using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Extensions;
using DotnetDocument.Utils;
using Humanizer;

namespace DotnetDocument.Format
{
    /// <summary>
    /// The humanize formatter class
    /// </summary>
    /// <seealso cref="IFormatter" />
    public class HumanizeFormatter : IFormatter
    {
        /// <summary>
        /// The options
        /// </summary>
        private readonly DocumentationOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="HumanizeFormatter" /> class
        /// </summary>
        /// <param name="options">The options</param>
        public HumanizeFormatter(DocumentationOptions options) =>
            this.options = options;

        /// <summary>
        /// Formats the name using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="names">The names</param>
        /// <returns>The string</returns>
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

        /// <summary>
        /// Formats the inherits using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="key">The key</param>
        /// <param name="names">The names</param>
        /// <returns>The inherits</returns>
        public string FormatInherits(string template, string key, params string[] names)
        {
            var inheritsFromDescription = string.Join(" and ", names);

            var inherits = template.Replace(key, inheritsFromDescription);

            return inherits;
        }

        /// <summary>
        /// Conjugates the third person singular using the specified verb
        /// </summary>
        /// <param name="verb">The verb</param>
        /// <returns>The string</returns>
        public string ConjugateThirdPersonSingular(string verb)
        {
            // Check if there is a custom verb conjugation
            if (options.Verbs.ContainsKey(verb.ToLowerInvariant())) return options.Verbs[verb];

            return EnglishUtils.ConjugateToThirdPersonSingular(verb);
        }

        /// <summary>
        /// Formats the method using the specified method name
        /// </summary>
        /// <param name="methodName">The method name</param>
        /// <param name="returnType">The return type</param>
        /// <param name="modifiers">The modifiers</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="attributes">The attributes</param>
        /// <returns>The string</returns>
        public string FormatMethod(string methodName, string returnType, IEnumerable<string> modifiers,
            IEnumerable<string> parameters, IEnumerable<string> attributes)
        {
            // Humanize parameter names
            var humanizedParameters = parameters
                .Select(p => p.Humanize().ToLower())
                .ToList();

            // Remove suffixes
            var cleanAction = methodName
                .Replace("_", " ")
                .RemoveEnd(options.SuffixesToRemove);

            // Humanize the name to split word on case change
            var humanizedMethodName = cleanAction.Humanize().ToLower();

            // Identify the words that compose the method name
            var words = humanizedMethodName.Split(" ");

            // Take the first word and format it as a verb
            var verb = ConjugateThirdPersonSingular(words[0]);

            // Check if method is test
            var isTestMethod = attributes.Any(a => options.TestAttributes.Contains(a));

            // Check if method is static
            var isStaticMethod = modifiers.Any(m => m == "static");

            // Humanize the return type
            var humanizedReturnType = FormatUtils.HumanizeReturnsType(returnType);

            switch (returns: humanizedReturnType, @static: isStaticMethod, test: isTestMethod, wordsCount: words.Length,
                parametersCount: humanizedParameters.Count)
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
                        .Replace(TemplateKeys.FirstParam, humanizedParameters[0])
                        .FirstCharToUpper();

                // Multiple words method with params
                case { wordsCount: > 1, parametersCount: > 0 }:
                    return options.Method.Summary.ManyArgsManyWordMethod
                        .Replace(TemplateKeys.Verb, verb)
                        .Replace(TemplateKeys.Object, string.Join(" ", words.Skip(1)))
                        .Replace(TemplateKeys.FirstParam, humanizedParameters[0])
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
