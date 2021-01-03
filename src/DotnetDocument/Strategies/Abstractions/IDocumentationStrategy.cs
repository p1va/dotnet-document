using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public interface IDocumentationStrategy
    {
        IEnumerable<SyntaxKind> GetSupportedKinds();
        SyntaxNode Apply(SyntaxNode node);
    }
}
