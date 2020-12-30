using System.Linq;
using DotnetDocument.Strategies.Abstractions;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace DotnetDocument.Strategies
{
    public class MethodDeclarationDocumentationStrategy : DocumentationStrategyBase<MethodDeclarationSyntax>
    {
        public override SyntaxKind GetKind() => SyntaxKind.MethodDeclaration;

        public override MethodDeclarationSyntax Apply(MethodDeclarationSyntax node)
        {
            var builder = GetDocumentationBuilder()
                .For(node);

            var methodName = node.Identifier.Text;

            var summary = $"{methodName.Humanize()}";

            builder.WithSummary(summary);

            if (node.Body is not null)
            {
                var blockComments = node.Body?
                    .DescendantTrivia()
                    .Where(t => CSharpExtensions.Kind((SyntaxTrivia)t) == SyntaxKind.SingleLineCommentTrivia)
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

            if (node.TypeParameterList is not null)
            {
                foreach (var typeParamSyntax in node.TypeParameterList.Parameters)
                {
                    var paramName = typeParamSyntax.Identifier.Text;
                    var paramDescription = $"The {paramName.Humanize()}";

                    builder.WithTypeParam(paramName, paramDescription);
                }
            }

            foreach (var paramSyntax in node.ParameterList.Parameters)
            {
                var paramName = paramSyntax.Identifier.Text;
                var paramDescription = $"The {paramName.Humanize().ToLower()}";

                builder.WithParam(paramName, paramDescription);
            }

            return builder.Build();
        }
    }
}
