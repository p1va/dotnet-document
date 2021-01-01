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
    public class ClassDocumentationStrategy : DocumentationStrategyBase<ClassDeclarationSyntax>
    {
        private readonly ILogger<ClassDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public ClassDocumentationStrategy(ILogger<ClassDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Class);

        public override SyntaxKind GetKind() => SyntaxKind.ClassDeclaration;

        public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node)
        {
            // Retrieve class name
            var className = node.Identifier.Text;

            // Declare the summary by using the template from configuration
            var summary = new List<string>
            {
                _formatter.FormatName(_options.Summary.Template, ("{{name}}", className))
            };

            // If inheritance has to be included
            if (_options.Summary.IncludeInheritance)
            {
                // Retrieve base types and use the template to format summary lines
                var baseTypes = SyntaxUtils
                    .ExtractBaseTypes(node)
                    .ToList();

                if (baseTypes.Any())
                {
                    //_logger.LogDebug("The following inherits lines will be added to summary: {Lines}", baseTypes);

                    var inheritsFromDescription = _formatter.FormatInherits(
                        _options.Summary.InheritanceTemplate, "{{name}}", baseTypes.ToArray());

                    summary.Add(inheritsFromDescription);
                }
            }

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary)
                .Build();
        }
    }
}
