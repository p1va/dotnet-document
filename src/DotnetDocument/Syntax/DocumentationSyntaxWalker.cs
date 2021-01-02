using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Syntax
{
    public class DocumentationSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly IEnumerable<SyntaxKind> _kinds;

        public DocumentationSyntaxWalker(IEnumerable<SyntaxKind> kinds) => (_kinds) = (kinds);

        public IList<SyntaxNode> NodesWithXmlDoc { get; } = new List<SyntaxNode>();

        public IList<SyntaxNode> NodesWithoutXmlDoc { get; } = new List<SyntaxNode>();

        public void Clean()
        {
            NodesWithXmlDoc.Clear();
            NodesWithoutXmlDoc.Clear();
        }

        private bool IsDocumentable(SyntaxKind kind) => _kinds.Any(k => k == kind);

        private void VisitCore(SyntaxNode node)
        {
            if (IsDocumentable(node.Kind()))
            {
                if (SyntaxUtils.IsDocumented(node))
                {
                    NodesWithXmlDoc.Add(node);
                }
                else
                {
                    NodesWithoutXmlDoc.Add(node);
                }
            }

            base.DefaultVisit(node);
        }

        public override void DefaultVisit(SyntaxNode node) => VisitCore(node);

        /*
        /// <summary>Called when the visitor visits a ClassDeclarationSyntax node.</summary>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a RecordDeclarationSyntax node.</summary>
        public override void VisitRecordDeclaration(RecordDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a ConstructorDeclarationSyntax node.</summary>
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a DestructorDeclarationSyntax node.</summary>
        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a EnumDeclarationSyntax node.</summary>
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a EventDeclarationSyntax node.</summary>
        public override void VisitEventDeclaration(EventDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a FieldDeclarationSyntax node.</summary>
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a DelegateDeclarationSyntax node.</summary>
        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a IndexerDeclarationSyntax node.</summary>
        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a InterfaceDeclarationSyntax node.</summary>
        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a MethodDeclarationSyntax node.</summary>
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a PropertyDeclarationSyntax node.</summary>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a StructDeclarationSyntax node.</summary>
        public override void VisitStructDeclaration(StructDeclarationSyntax node) => VisitCore(node);

        /// <summary>Called when the visitor visits a EnumMemberDeclarationSyntax node.</summary>
        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node) => VisitCore(node);
        */
    }
}
