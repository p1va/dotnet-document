using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Tools.Handlers
{
    /// <summary>
    /// The member documentation status class
    /// </summary>
    public class MemberDocumentationStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDocumentationStatus" /> class
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="identifier">The identifier</param>
        /// <param name="kind">The kind</param>
        /// <param name="needsDocumentation">The is documented</param>
        /// <param name="nodeWithoutDocument">The node without document</param>
        /// <param name="documentedNode">The documented node</param>
        /// <param name="startLine">The start line</param>
        public MemberDocumentationStatus(string filePath, string identifier, SyntaxKind kind, bool needsDocumentation,
            SyntaxNode? nodeWithoutDocument, SyntaxNode? documentedNode, string startLine)
        {
            FilePath = filePath;
            Identifier = identifier;
            Kind = kind;
            NeedsDocumentation = needsDocumentation;
            NodeWithoutDocument = nodeWithoutDocument;
            DocumentedNode = documentedNode;
            StartLine = startLine;
        }

        /// <summary>
        /// Gets the value of the file path
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the value of the identifier
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the value of the kind
        /// </summary>
        public SyntaxKind Kind { get; }

        /// <summary>
        /// Gets or inits the value of the is documented
        /// </summary>
        public bool NeedsDocumentation { get; init; }

        /// <summary>
        /// Gets the value of the node without document
        /// </summary>
        public SyntaxNode? NodeWithoutDocument { get; }

        /// <summary>
        /// Gets the value of the documented node
        /// </summary>
        public SyntaxNode? DocumentedNode { get; }

        /// <summary>
        /// Gets the value of the start line
        /// </summary>
        public string StartLine { get; }
    }
}
