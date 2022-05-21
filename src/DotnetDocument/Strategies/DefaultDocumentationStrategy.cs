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

namespace DotnetDocument.Strategies
{
    /// <summary>
    /// The default documentation strategy class
    /// </summary>
    /// <seealso cref="DocumentationStrategyBase{T}" />
    [Strategy(nameof(SyntaxKind.FieldDeclaration))]
    [Strategy(nameof(SyntaxKind.DelegateDeclaration))]
    [Strategy(nameof(SyntaxKind.EventDeclaration))]
    [Strategy(nameof(SyntaxKind.IndexerDeclaration))]
    [Strategy(nameof(SyntaxKind.RecordDeclaration))]
    [Strategy(nameof(SyntaxKind.StructDeclaration))]
    public class DefaultDocumentationStrategy : DocumentationStrategyBase<MemberDeclarationSyntax>
    {
        /// <summary>
        /// The formatter
        /// </summary>
        private readonly IFormatter _formatter;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DefaultDocumentationStrategy> _logger;

        /// <summary>
        /// The options
        /// </summary>
        private readonly DefaultMemberDocumentationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDocumentationStrategy" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="formatter">The formatter</param>
        /// <param name="options">The options</param>
        public DefaultDocumentationStrategy(ILogger<DefaultDocumentationStrategy> logger,
            IFormatter formatter, DefaultMemberDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.FieldDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.StructDeclaration
        };

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The member declaration syntax</returns>
        public override (bool IsChanged, MemberDeclarationSyntax NodeWithDocs) Apply(MemberDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node);

            var memberModifiers = node.Modifiers.Select(m => m.Text);
            var allowedModifiers = _options.ApplyOnModifiers;

            if (memberModifiers.Any(m => allowedModifiers.Contains(m)) is false)
            {
                return (false, node);
            }

            // Retrieve member name
            var name = SyntaxUtils.FindMemberIdentifier(node);

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template,
                    (TemplateKeys.Name, name))
            };

            var nodeWithDocs = GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();

            return (true, nodeWithDocs);
        }
    }
}
