using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Tools.Handlers
{
    public class MemberDocumentationStatus
    {
        public MemberDocumentationStatus(string filePath, string identifier, SyntaxKind kind, bool isDocumented,
            SyntaxNode? nodeWithoutDocument, SyntaxNode? documentedNode, string startLine)
        {
            FilePath = filePath;
            Identifier = identifier;
            Kind = kind;
            IsDocumented = isDocumented;
            NodeWithoutDocument = nodeWithoutDocument;
            DocumentedNode = documentedNode;
            StartLine = startLine;
        }

        public string FilePath { get; }
        public string Identifier { get; }
        public SyntaxKind Kind { get; }
        public bool IsDocumented { get; init; }
        public SyntaxNode? NodeWithoutDocument { get; }
        public SyntaxNode? DocumentedNode { get; }
        public string StartLine { get; }
    }
}
