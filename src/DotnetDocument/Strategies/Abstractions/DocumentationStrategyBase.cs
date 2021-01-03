using System;
using System.Collections.Generic;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public abstract class DocumentationStrategyBase<T> : IDocumentationStrategy where T : SyntaxNode
    {
        protected abstract SyntaxKind GetKind();

        public virtual IEnumerable<SyntaxKind> GetKinds() => new List<SyntaxKind>
        {
            GetKind()
        };

        public SyntaxNode Apply(SyntaxNode node) => Apply(node as T);
        public abstract T Apply(T node);
        protected DocumentationBuilder<T> GetDocumentationBuilder() => new();
    }
}
