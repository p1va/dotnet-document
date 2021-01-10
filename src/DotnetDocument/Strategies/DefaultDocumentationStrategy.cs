using System.Collections.Generic;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.FieldDeclaration))]
    [Strategy(nameof(SyntaxKind.DelegateDeclaration))]
    [Strategy(nameof(SyntaxKind.EventDeclaration))]
    [Strategy(nameof(SyntaxKind.IndexerDeclaration))]
    [Strategy(nameof(SyntaxKind.RecordDeclaration))]
    [Strategy(nameof(SyntaxKind.StructDeclaration))]
    public class DefaultDocumentationStrategy : DocumentationStrategyBase<MemberDeclarationSyntax>
    {
        private readonly ILogger<DefaultDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DefaultMemberDocumentationOptions _options;

        public DefaultDocumentationStrategy(ILogger<DefaultDocumentationStrategy> logger,
            IFormatter formatter, DefaultMemberDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.FieldDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.StructDeclaration,
        };

        public override MemberDeclarationSyntax Apply(MemberDeclarationSyntax node)
        {
            // Retrieve member name
            var name = SyntaxUtils.FindMemberIdentifier(node);

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template,
                    (TemplateKeys.Name, name))
            };

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
