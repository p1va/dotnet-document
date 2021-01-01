using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Extensions;
using Humanizer;

namespace DotnetDocument.Format
{
    public interface IFormatter
    {
        string FormatName(string template, params (string key, string value)[] names);
        string FormatInherits(string template, string key, params string[] names);
        string FormatVerb(string verb);
        string FormatMethod(string name, string returnType, IEnumerable<string> parameters);
    }

    public class HumanizeFormatter : IFormatter
    {
        // TODO: Read from config
        private static readonly string[] _prefixesToRemove =
        {
            "I",
        };

        // TODO: Read from config
        private static readonly string[] _suffixToRemove =
        {
            "Class",
            "Async"
        };

        public string FormatName(string template, params (string key, string value)[] names)
        {
            var formattedName = template;

            // Remove suffixes
            foreach (var name in names)
            {
                // Humanize the name to split word on case change
                var humanizedName = name.value
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
            return formattedName.ToLower().Humanize();
        }

        public string FormatInherits(string template, string key, params string[] names)
        {
            var inheritsFromDescription = string.Join(" and ", names);

            var inherits = template.Replace(key, inheritsFromDescription);

            return inherits;
        }

        public string FormatVerb(string verb) => $"{verb}s";

        public string FormatMethod(string name, string returnType, IEnumerable<string> parameters)
        {
            var humanizedParameters = parameters
                .Select(p => p.Humanize().ToLower())
                .ToList();

            // Remove suffixes
            var cleanAction = name.RemoveEnd(_suffixToRemove);

            // Humanize the name to split word on case change
            var humanizedName = cleanAction.Humanize().ToLower();

            var words = humanizedName.Split(" ");

            var verb = FormatVerb(words.First());

            switch (returnType, wordsCount: words.Length, parametersCount: humanizedParameters.Count())
            {
                case { returnType: "bool" }:
                    return "Gets a flag describing whether this instance {{verb}}"
                        .Replace("{{verb}}", humanizedName)
                        .Humanize();

                case { wordsCount: 1, parametersCount: 0 }:
                    return "{{verb}} this instance"
                        .Replace("{{verb}}", verb)
                        .Humanize();

                case { wordsCount: 1, parametersCount: > 0 }:
                    return "{{verb}} the {{parameter}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{parameter}}", humanizedParameters.First())
                        .Humanize();

                case { wordsCount: > 1, parametersCount: > 0 }:
                    return "{{verb}} the {{object}} using the specified {{parameter}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{object}}", string.Join(" ", words.Skip(1)))
                        .Replace("{{parameter}}", humanizedParameters.First())
                        .Humanize();

                default:
                    return "{{verb}} the {{object}}"
                        .Replace("{{verb}}", verb)
                        .Replace("{{object}}", string.Join(" ", words.Skip(1)))
                        .Humanize();
            }
        }
    }
}
