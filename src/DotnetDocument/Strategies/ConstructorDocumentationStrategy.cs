using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    public class ConstructorDocumentationStrategy : DocumentationStrategyBase<ConstructorDeclarationSyntax>
    {
        private readonly ILogger<ConstructorDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public ConstructorDocumentationStrategy(ILogger<ConstructorDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Constructor);

        public override SyntaxKind GetKind() => SyntaxKind.ConstructorDeclaration;

        public override ConstructorDeclarationSyntax Apply(ConstructorDeclarationSyntax node)
        {
            // Retrieve constructor name
            var ctorName = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template, ("{{name}}", ctorName))
            };

            var exceptions = new List<(string, string)>();

            // Check if constructor has a block body {...}
            if (node.Body is not null)
            {
                exceptions.AddRange(
                    DocumentationSyntaxUtils.GetThrownExceptions(node.Body));
            }

            // Check if constructor has an expression body => {...}
            if (node.ExpressionBody is not null)
            {
            }

            // Extract params and generate a description
            var @params = DocumentationSyntaxUtils
                .ExtractParams(node.ParameterList)
                .Select(p => (p, $"The {p.Humanize()}"));

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .WithParams(@params)
                .WithExceptions(exceptions.ToArray())
                .Build();
        }
    }
}
