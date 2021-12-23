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
    /// <summary>
    /// The constructor documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.ConstructorDeclaration))]
    public class ConstructorDocumentationStrategy : DocumentationStrategyBase<ConstructorDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ConstructorDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly CtorDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public ConstructorDocumentationStrategy(ILogger<ConstructorDocumentationStrategy> logger,
            IFormatter formatter, CtorDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.ConstructorDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The constructor declaration syntax</returns>
        public override ConstructorDeclarationSyntax Apply(ConstructorDeclarationSyntax node)
        {
            // Retrieve constructor name
            var ctorName = SyntaxUtils.ExtractClassName(node);

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _options.Summary.Template.Replace(TemplateKeys.Name, $"<<{ctorName}>>")
            };

            var exceptions = new List<(string, string)>();

            if (_options.Exceptions.Enabled)
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
                    .FormatName(_options.Parameters.Template, (TemplateKeys.Name, p))));

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .WithParams(@params)
                .WithExceptions(exceptions.ToArray())
                .Build();
        }
    }
}
