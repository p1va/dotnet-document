using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public interface IDocumentationStrategy
    {
        SyntaxKind GetKind();
        SyntaxNode Apply(SyntaxNode node);
        public delegate IDocumentationStrategy ServiceResolver(SyntaxKind key);
    }
}
