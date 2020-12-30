using System;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    public abstract class DocumentationStrategyBase<T> : IDocumentationStrategy where T : SyntaxNode
    {
        public abstract SyntaxKind GetKind();
        public Type GetNodeType() => typeof(T);
        public SyntaxNode Apply(SyntaxNode node) => Apply(node as T);
        public abstract T Apply(T node);
        protected DocumentationBuilder<T> GetDocumentationBuilder() => new();
    }
}
