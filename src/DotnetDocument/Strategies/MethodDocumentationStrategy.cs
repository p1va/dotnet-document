using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    public class MethodDocumentationStrategy : DocumentationStrategyBase<MethodDeclarationSyntax>
    {
        private readonly ILogger<MethodDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public MethodDocumentationStrategy(ILogger<MethodDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Method);

        public override SyntaxKind GetKind() => SyntaxKind.MethodDeclaration;

        public override MethodDeclarationSyntax Apply(MethodDeclarationSyntax node)
        {
            var builder = GetDocumentationBuilder()
                .For(node);

            var methodName = node.Identifier.Text;

            // Extract type params and generate a description
            var typeParams = DocumentationSyntaxUtils
                .ExtractTypeParams(node.TypeParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.TypeParameters.Template, ("{{name}}", p))));

            // Extract params and generate a description
            var @params = DocumentationSyntaxUtils
                .ExtractParams(node.ParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.Parameters.Template, ("{{name}}", p))));

            // Extract return type
            var returnType = node.ReturnType.ToString();

            var summary = _formatter.FormatMethod(methodName, returnType, @params.Select(p => p.p));

            builder.WithSummary(summary);

            if (node.Body is not null)
            {
                var blockComments = node.Body?
                    .DescendantTrivia()
                    .Where(t => t.Kind() == SyntaxKind.SingleLineCommentTrivia)
                    .Select(t => t.ToFullString().Replace("//", string.Empty).Trim())
                    .ToArray();

                if (blockComments is not null)
                {
                    builder.WithSummary(blockComments);
                }

                var throwStatments = node.Body.Statements.OfType<ThrowStatementSyntax>();

                foreach (var throwStatment in throwStatments)
                {
                    var exceptionType = string.Empty;
                    var exceptionMessage = string.Empty;

                    if (throwStatment.Expression is ObjectCreationExpressionSyntax exceptionCreation)
                    {
                        exceptionType = exceptionCreation.Type.ToFullString();

                        var exceptionCreationArg = exceptionCreation.ArgumentList?.Arguments.FirstOrDefault();

                        if (exceptionCreationArg?.Expression is LiteralExpressionSyntax literalExpressionSyntax)
                        {
                            exceptionMessage = literalExpressionSyntax.Token.ValueText;
                        }

                        if (exceptionCreationArg?.Expression is InterpolatedStringExpressionSyntax
                            interpolatedStringExpressionSyntax)
                        {
                            var contents = interpolatedStringExpressionSyntax.Contents.Select(c => c.ToFullString());
                            exceptionMessage = string.Join(string.Empty, contents);
                        }
                    }

                    builder.WithException(exceptionType, exceptionMessage);
                }

                var returns = node.Body.Statements.OfType<ReturnStatementSyntax>();

                foreach (var returnStatement in returns)
                {
                    if (returnStatement.Expression is IdentifierNameSyntax identifierNameSyntax)
                    {
                        var description = identifierNameSyntax.Identifier.Text.Humanize().ToLower();
                        builder.WithReturns($"The {description}");
                    }
                }
            }

            return builder
                .WithTypeParams(typeParams)
                .WithParams(@params)
                .Build();
        }
    }
}
