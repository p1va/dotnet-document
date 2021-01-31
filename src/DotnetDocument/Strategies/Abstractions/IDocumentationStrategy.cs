using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Strategies.Abstractions
{
    /// <summary>
    /// The documentation strategy interface
    /// </summary>
    public interface IDocumentationStrategy
    {
        /// <summary>
        /// Gets the supported kinds
        /// </summary>
        /// <returns>An enumerable of syntax kind</returns>
        IEnumerable<SyntaxKind> GetSupportedKinds();

        /// <summary>
        /// Applies the node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The syntax node</returns>
        SyntaxNode Apply(SyntaxNode node);
    }
}
