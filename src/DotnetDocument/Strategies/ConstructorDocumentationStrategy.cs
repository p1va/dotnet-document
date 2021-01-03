using System;
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
    [Strategy(nameof(SyntaxKind.ConstructorDeclaration))]
    public class ConstructorDocumentationStrategy : DocumentationStrategyBase<ConstructorDeclarationSyntax>
    {
        private readonly ILogger<ConstructorDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public ConstructorDocumentationStrategy(ILogger<ConstructorDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Constructor);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.ConstructorDeclaration
        };

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

            if (_options.Exceptions.Enable)
            {
                // Check if constructor has a block body {...}
                if (node.Body is not null)
                {
                    // Extract exceptions from body
                    var extractedExceptions = SyntaxUtils.ExtractThrownExceptions(node.Body).ToList();

                    // Sort them
                    extractedExceptions.Sort((p, n) => string.CompareOrdinal(p.type, n.type));
                    extractedExceptions.Sort((p, n) => string.CompareOrdinal(p.message, n.message));

                    exceptions.AddRange(extractedExceptions);
                }

                // Check if constructor has an expression body => {...}
                if (node.ExpressionBody is not null)
                {
                    // TODO: Extract exceptions in lambda
                }
            }

            // Extract params and generate a description
            var @params = SyntaxUtils
                .ExtractParams(node.ParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.Parameters.Template, ("{{name}}", p))));

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .WithParams(@params)
                .WithExceptions(exceptions.ToArray())
                .Build();
        }
    }
}
