using DotnetDocument.Strategies.Abstractions;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Strategies
{
    public class ConstructorDeclarationDocumentationStrategy : DocumentationStrategyBase<ConstructorDeclarationSyntax>
    {
        public override SyntaxKind GetKind() => SyntaxKind.ConstructorDeclaration;

        public override ConstructorDeclarationSyntax Apply(ConstructorDeclarationSyntax node)
        {
            var className = node.Identifier.Text;

            var summary = $"Creates a new instance of the {className.Humanize().ToLower()} class";

            //node.BaseList.Types.Select(t => t.GetText());

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary)
                .Build();
        }
    }
}
