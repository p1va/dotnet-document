using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.EnumDeclaration))]
    public class EnumDocumentationStrategy : DocumentationStrategyBase<EnumDeclarationSyntax>
    {
        private readonly ILogger<EnumDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public EnumDocumentationStrategy(ILogger<EnumDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Enum);

        protected override SyntaxKind GetKind() => SyntaxKind.EnumDeclaration;

        public override EnumDeclarationSyntax Apply(EnumDeclarationSyntax node)
        {
            // Retrieve class name
            var enumName = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template, ("{{name}}", enumName))
            };

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
