using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public static class DocumentationUtils
    {
        public static SyntaxTrivia GetIndentationTrivia(SyntaxNode node)
        {
            var leadingTrivia = node
                .GetLeadingTrivia();

            var indentationTrivia = leadingTrivia
                .Last();

            return indentationTrivia;
        }

        public static SyntaxTrivia GetIndentationElement(SyntaxNode node)
        {
            var leadingTrivia = node
                .GetLeadingTrivia();

            var indentationTrivia = leadingTrivia
                .Last();

            return indentationTrivia;
        }

        public static IList<DocumentationCommentTriviaSyntax> GetXmlDocuments(SyntaxNode node) => node
            .GetLeadingTrivia()
            .Select(s => s.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .ToList();

        public static bool IsDocumented(SyntaxNode node) =>
            GetXmlDocuments(node).Any();

        private static string FindMemberIdentifier(SyntaxNode node)
        {
            var directNodeIdentifier = node
                .ChildTokens()
                .FirstOrDefault(t => t.Kind() == SyntaxKind.IdentifierToken)
                .Text;

            if (!string.IsNullOrWhiteSpace(directNodeIdentifier))
            {
                return directNodeIdentifier;
            }

            var descendantIdentifier = node
                .DescendantTokens()
                .FirstOrDefault(t => t.Kind() == SyntaxKind.IdentifierToken)
                .Text;

            return descendantIdentifier;
        }
    }
}
