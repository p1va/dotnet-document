using System;
using System.Collections.Generic;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Strategies.Abstractions
{
    /// <summary>
    /// The documentation strategy base class
    /// </summary>
    /// <seealso cref="IDocumentationStrategy" />
    public abstract class DocumentationStrategyBase<T> : IDocumentationStrategy where T : SyntaxNode
    {
        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        public abstract IEnumerable<SyntaxKind> GetSupportedKinds();

        public bool ShouldDocument(SyntaxNode node) =>
            ShouldDocument(node as T ?? throw new InvalidOperationException());

        public abstract bool ShouldDocument(T node);
        
        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The syntax node</returns>
        public SyntaxNode Apply(SyntaxNode node) => Apply(node as T ?? throw new InvalidOperationException());

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The</returns>
        public abstract T Apply(T node);

        /// <summary>
        /// Gets the documentation builder
        /// </summary>
        /// <returns>A documentation builder of t</returns>
        protected DocumentationBuilder<T> GetDocumentationBuilder() => new();
    }
}
