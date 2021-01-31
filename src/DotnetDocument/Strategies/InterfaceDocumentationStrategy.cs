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
    /// The interface documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.InterfaceDeclaration))]
    public class InterfaceDocumentationStrategy : DocumentationStrategyBase<InterfaceDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<InterfaceDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly InterfaceDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public InterfaceDocumentationStrategy(ILogger<InterfaceDocumentationStrategy> logger,
            IFormatter formatter, InterfaceDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.InterfaceDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The interface declaration syntax</returns>
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
