using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public interface IDocumentationStrategy
    {
        IEnumerable<SyntaxKind> GetKinds();
        SyntaxNode Apply(SyntaxNode node);
    }
}
