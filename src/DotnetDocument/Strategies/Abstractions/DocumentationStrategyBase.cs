using System;
using System.Collections.Generic;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The bool is changed syntax node node with docs</returns>
        public (bool IsChanged, SyntaxNode NodeWithDocs) Apply(SyntaxNode node) =>
            Apply(node as T ?? throw new InvalidOperationException());

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The bool is changed node with docs</returns>
        public abstract (bool IsChanged, T NodeWithDocs) Apply(T node);

        /// <summary>
        /// Gets the documentation builder
        /// </summary>
        /// <returns>A documentation builder of t</returns>
        protected DocumentationBuilder<T> GetDocumentationBuilder() => new();
    }
}
