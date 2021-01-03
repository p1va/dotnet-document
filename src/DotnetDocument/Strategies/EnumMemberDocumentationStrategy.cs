using System.Collections.Generic;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.EnumMemberDeclaration))]
    public class EnumMemberDocumentationStrategy : DocumentationStrategyBase<EnumMemberDeclarationSyntax>
    {
        private readonly ILogger<EnumMemberDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public EnumMemberDocumentationStrategy(ILogger<EnumMemberDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.EnumMember);

        protected override SyntaxKind GetKind() => SyntaxKind.EnumMemberDeclaration;

        public override EnumMemberDeclarationSyntax Apply(EnumMemberDeclarationSyntax node)
        {
            // Retrieve class name
            var enumMemberName = node.Identifier.Text;
            var enumName = string.Empty;

            if (node.Parent is EnumDeclarationSyntax enumDeclaration)
            {
                enumName = enumDeclaration.Identifier.Text;
            }

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template,
                    ("{{name}}", enumMemberName),
                    ("{{enum-name}}", enumName))
            };

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
