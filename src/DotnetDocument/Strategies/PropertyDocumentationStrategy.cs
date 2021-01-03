using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Strategies.Abstractions;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.PropertyDeclaration))]
    public class PropertyDocumentationStrategy : DocumentationStrategyBase<PropertyDeclarationSyntax>
    {
        private readonly ILogger<PropertyDocumentationStrategy> _logger;
        private readonly DeclarationDocOptions _options;

        public PropertyDocumentationStrategy(ILogger<PropertyDocumentationStrategy> logger,
            IOptions<DotnetDocumentOptions> options) =>
            (_logger, _options) = (logger, options.Value.Property);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.PropertyDeclaration
        };

        public override PropertyDeclarationSyntax Apply(PropertyDeclarationSyntax node)
        {
            // Retrieve constructor name
            var propertyName = node.Identifier.Text;

            // Humanize the constructor name
            var humanizedPropertyName = propertyName.Humanize().ToLower();

            var accessorsDescription = "";

            if (node.AccessorList is not null)
            {
                var accessors = node.AccessorList?.Accessors
                    .Select(a => $"{a.Keyword.Text}s")
                    .ToList();

                if (accessors.Any())
                {
                    accessorsDescription = string.Join(" or ", accessors)
                        .ToLower()
                        .Humanize();
                }
                else
                {
                    accessorsDescription = "Gets";
                }
            }

            var summary = new List<string>
            {
                // Declare the summary by using the template from configuration
                _options.Summary.Template
                    .Replace("{{accessors}}", accessorsDescription)
                    .Replace("{{name}}", humanizedPropertyName)
            };

            // Check if constructor has an expression body => {...}
            if (node.ExpressionBody is not null)
            {
            }

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
