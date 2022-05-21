using System;
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
    /// The enum member documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.EnumMemberDeclaration))]
    public class EnumMemberDocumentationStrategy : DocumentationStrategyBase<EnumMemberDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<EnumMemberDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly EnumMemberDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumMemberDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public EnumMemberDocumentationStrategy(ILogger<EnumMemberDocumentationStrategy> logger,
            IFormatter formatter, EnumMemberDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.EnumMemberDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The enum member declaration syntax</returns>
        public override (bool IsChanged, EnumMemberDeclarationSyntax NodeWithDocs) Apply(
            EnumMemberDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node);

            // Retrieve class name
            var enumMemberName = node.Identifier.Text;
            var enumName = string.Empty;

            if (node.Parent is EnumDeclarationSyntax enumDeclaration) enumName = enumDeclaration.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template,
                    (TemplateKeys.Name, enumMemberName),
                    (TemplateKeys.EnumName, enumName))
            };

            var nodeWithDocs = GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();

            return (true, nodeWithDocs);
        }
    }
}
