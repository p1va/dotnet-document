using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Tools.Commands
{
    public class MemberDocumentationStatus
    {
        public string FilePath { get; set; }
        public string Identifier { get; set; }
        public SyntaxKind Kind { get; set; }
        public bool IsDocumented { get; set; }
        public SyntaxNode NodeWithoutDocument { get; set; }
        public SyntaxNode DocumentedNode { get; set; }
        public string StartLine { get; set; }
    }
}
