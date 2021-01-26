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
    [Strategy(nameof(SyntaxKind.ClassDeclaration))]
    public class ClassDocumentationStrategy : DocumentationStrategyBase<ClassDeclarationSyntax>
    {
        private readonly ILogger<ClassDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly ClassDocumentationOptions _options;

        public ClassDocumentationStrategy(ILogger<ClassDocumentationStrategy> logger,
            IFormatter formatter, ClassDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.ClassDeclaration
        };

        public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node)
        {
            // Retrieve class name
            var className = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = _formatter.FormatName(_options.Summary.Template,
                (TemplateKeys.Name, className));

            // Get the builder for the node
            var builder = GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary);

            // If inheritance has to be included
            if (_options.Summary.IncludeInheritance)
            {
                // Retrieve base types
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
