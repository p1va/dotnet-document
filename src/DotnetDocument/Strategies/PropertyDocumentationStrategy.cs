using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies
{
    /// <summary>
    /// The property documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.PropertyDeclaration))]
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public class PropertyDocumentationStrategy : DocumentationStrategyBase<PropertyDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PropertyDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly PropertyDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public PropertyDocumentationStrategy(ILogger<PropertyDocumentationStrategy> logger,
            IFormatter formatter, PropertyDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.PropertyDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The property declaration syntax</returns>
        public override (bool IsChanged, PropertyDeclarationSyntax NodeWithDocs) Apply(PropertyDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node);

            var memberModifiers = node.Modifiers.Select(m => m.Text);
            var allowedModifiers = _options.ApplyOnModifiers;

            if (memberModifiers.Any(m => allowedModifiers.Contains(m)) is false)
            {
                return (false, node);
            }

            // Retrieve constructor name
            var propertyName = node.Identifier.Text;

            // Humanize the constructor name
            var humanizedPropertyName = propertyName.Humanize().ToLowerInvariant();

            var accessorsDescription = "";

            var accessors = node.AccessorList?.Accessors
                .Select(a => _formatter.ConjugateThirdPersonSingular(a.Keyword.Text))
                .ToList();

            if (accessors is not null && accessors.Any())
            {
                accessorsDescription = string.Join(" or ", accessors)
                    .ToLowerInvariant()
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
                    .Replace(TemplateKeys.Accessors, accessorsDescription, StringComparison.InvariantCulture)
                    .Replace(TemplateKeys.Name, humanizedPropertyName, StringComparison.InvariantCulture)
            };

            // Check if constructor has an expression body => {...}
            if (node.ExpressionBody is not null)
            {
            }

            var nodeWithDocs = GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();

            return (true, nodeWithDocs);
        }
    }
}
