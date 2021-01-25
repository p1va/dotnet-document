using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.MethodDeclaration))]
    public class MethodDocumentationStrategy : DocumentationStrategyBase<MethodDeclarationSyntax>
    {
        private readonly ILogger<MethodDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly MethodDocumentationOptions _options;

        public MethodDocumentationStrategy(ILogger<MethodDocumentationStrategy> logger,
            IFormatter formatter, MethodDocumentationOptions options) =>
            (_logger, _formatter, _options) = (logger, formatter, options);

        public override IEnumerable<SyntaxKind> GetSupportedKinds() => new[]
        {
            SyntaxKind.MethodDeclaration
        };

        public override MethodDeclarationSyntax Apply(MethodDeclarationSyntax node)
        {
            // Get the doc builder for this node
            var builder = GetDocumentationBuilder()
                .For(node);

            // Extract method name
            var methodName = node.Identifier.Text;

            // Extract return type
            var returnType = node.ReturnType.ToString();

            if (returnType != "void" && returnType != "Task")
            {
                // Default returns description is empty
                var returns = string.Empty;

                if (node.Body is not null)
                {
                    // Extract the last return statement which returns a variable
                    // and humanize the name of the variable which will be used as
                    // returns descriptions. Empty otherwise.
                    returns = SyntaxUtils
                        .ExtractReturnStatements(node.Body)
                        .Select(r => _formatter
                            .FormatName(_options.Returns.Template, (TemplateKeys.Name, r)))
                        .LastOrDefault();
                }

                // TODO: Handle case where node.ExpressionBody is not null

                // In case nothing was found,
                // Humanize return type to get a description
                if (string.IsNullOrWhiteSpace(returns))
                {
                    // Humanize the return type
                    returns = FormatUtils.HumanizeReturnsType(returnType);
                }

                builder.WithReturns(returns);
            }

            // Extract type params and generate a description
            var typeParams = SyntaxUtils
                .ExtractTypeParams(node.TypeParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.TypeParameters.Template, (TemplateKeys.Name, p))));

            // Extract params and generate a description
            var @params = SyntaxUtils
                .ExtractParams(node.ParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.Parameters.Template, (TemplateKeys.Name, p))))
                .ToList();

            // Retrieve method attributes like [Theory], [Fact]
            var attributes = node.AttributeLists
                .SelectMany(a => a.Attributes)
                .Select(a => a.Name
                    .ToString()
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty));

            // Retrieve method modifiers like static, public, protected, async
            var modifiers = node.Modifiers.Select(m => m.ToString());

            // Format the summary for this method
            var summary = _formatter.FormatMethod(
                methodName, returnType, modifiers, @params.Select(p => p.p), attributes);

            builder.WithSummary(summary);

            // Check if single lines comments present in the body block
            // need to be included in the summary of the method 
            if (_options.Summary.IncludeComments)
            {
                var blockComments = SyntaxUtils.ExtractBlockComments(node.Body);

                builder.WithSummary(blockComments);
            }

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

                    builder.WithExceptions(extractedExceptions);
                }

                // Check if constructor has an expression body => {...}
                if (node.ExpressionBody is not null)
                {
                    // TODO: Extract exceptions in lambda
                }
            }

            return builder
                .WithTypeParams(typeParams)
                .WithParams(@params)
                .Build();
        }
    }
}
