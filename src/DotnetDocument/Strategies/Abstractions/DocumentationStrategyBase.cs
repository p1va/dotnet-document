using System;
using System.Collections.Generic;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public abstract class DocumentationStrategyBase<T> : IDocumentationStrategy where T : SyntaxNode
    {
        public abstract IEnumerable<SyntaxKind> GetSupportedKinds();
        public SyntaxNode Apply(SyntaxNode node) => Apply(node as T);
        public abstract T Apply(T node);
        protected DocumentationBuilder<T> GetDocumentationBuilder() => new();
    }
}
