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
    /// The class documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.ClassDeclaration))]
    public class ClassDocumentationStrategy : DocumentationStrategyBase<ClassDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ClassDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly ClassDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public ClassDocumentationStrategy(ILogger<ClassDocumentationStrategy> logger,
            IFormatter formatter, ClassDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.ClassDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The class declaration syntax</returns>
        public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node)
        {
            // Retrieve class name
            var className = node.Identifier.Text;

            // Check if we want to exclude private classess
            if (_options.ExcludePrivate)
            {
                _logger.LogInformation($"Configured to not generate documentation for private classes. Skipping {className}");

                if (node.Modifiers.Any((m) => m.Text.Contains("private")))
                {
                    // Just return the node as is to prevent us from adding docs.
                    return node;
                }
            }

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
