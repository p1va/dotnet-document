using System.Collections.Generic;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies
{
    /// <summary>
    /// The enum documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.EnumDeclaration))]
    public class EnumDocumentationStrategy : DocumentationStrategyBase<EnumDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<EnumDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly EnumDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public EnumDocumentationStrategy(ILogger<EnumDocumentationStrategy> logger,
            IFormatter formatter, EnumDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.EnumDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The enum declaration syntax</returns>
        public override EnumDeclarationSyntax Apply(EnumDeclarationSyntax node)
        {
            // Retrieve class name
            var enumName = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template, (TemplateKeys.Name, enumName))
            };

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
