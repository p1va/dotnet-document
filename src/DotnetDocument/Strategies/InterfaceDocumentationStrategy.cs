using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.InterfaceDeclaration))]
    public class InterfaceDocumentationStrategy : DocumentationStrategyBase<InterfaceDeclarationSyntax>
    {
        private readonly ILogger<InterfaceDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly InterfaceDocumentationOptions _options;

        public InterfaceDocumentationStrategy(ILogger<InterfaceDocumentationStrategy> logger,
            IFormatter formatter, InterfaceDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.InterfaceDeclaration
        };

        public override InterfaceDeclarationSyntax Apply(InterfaceDeclarationSyntax node)
        {
            // Retrieve class name
            var interfaceName = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = _formatter.FormatName(_options.Summary.Template, (TemplateKeys.Name, interfaceName));

            // Get the builder for this node
            var builder = GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary);

            // If inheritance has to be included
            if (_options.Summary.IncludeInheritance)
            {
                // Retrieve base types for the interface
                var baseTypes = SyntaxUtils
                    .ExtractBaseTypes(node)
                    .ToList();

                // Add them as see also elements
                builder.WithSeeAlso(baseTypes);
            }

            return builder.Build();
        }
    }
}
