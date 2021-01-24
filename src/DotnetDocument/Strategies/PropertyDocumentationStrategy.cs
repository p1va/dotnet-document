using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
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
        private readonly IFormatter _formatter;
        private readonly PropertyDocumentationOptions _options;

        public PropertyDocumentationStrategy(ILogger<PropertyDocumentationStrategy> logger,
            IFormatter formatter, PropertyDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

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

            var accessors = node.AccessorList?.Accessors
                .Select(a => _formatter.ConjugateThirdPersonSingular(a.Keyword.Text))
                .ToList();

            if (accessors is not null && accessors.Any())
            {
                accessorsDescription = string.Join(" or ", accessors)
                    .ToLower()
                    .Humanize();
            }
            else
            {
                accessorsDescription = _formatter.ConjugateThirdPersonSingular("Get");
            }

            var summary = new List<string>
            {
                // Declare the summary by using the template from configuration
                _options.Summary.Template
                    .Replace(TemplateKeys.Accessors, accessorsDescription)
                    .Replace(TemplateKeys.Name, humanizedPropertyName)
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
