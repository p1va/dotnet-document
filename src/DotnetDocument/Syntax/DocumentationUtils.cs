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

        public static SyntaxNode Document(SyntaxNode memberDeclarationSyntax)
        {
            var leadingTrivia = memberDeclarationSyntax
                .GetLeadingTrivia();

            var indentationTrivia = leadingTrivia
                .Last();

            // Get the identifier
            var identifier = FindMemberIdentifier(memberDeclarationSyntax);

            // Get the humanized description
            var humanized = identifier.Humanize();

            var indentedNewXmlLine = SyntaxFactory
                .XmlTextNewLine("\n", false)
                .WithTrailingTrivia(SyntaxFactory.TriviaList(
                    indentationTrivia, SyntaxFactory
                        .DocumentationCommentExterior("/// ")));

            // Declare the summary XML element
            var summaryXmlElement = SyntaxFactory
                .XmlSummaryElement(
                    SyntaxFactory
                        .XmlText(SyntaxFactory.TokenList(
                            indentedNewXmlLine,
                            SyntaxFactory.XmlTextLiteral(humanized),
                            SyntaxFactory.XmlTextLiteral(" ")
                        )),
                    SyntaxFactory
                        .XmlSeeElement(SyntaxFactory
                            .TypeCref(SyntaxFactory
                                .ParseTypeName("XmlDocGen.User"))),
                    SyntaxFactory
                        .XmlText(SyntaxFactory.TokenList(
                            indentedNewXmlLine,
                            SyntaxFactory.XmlTextLiteral("Inherits from BlaBla"),
                            indentedNewXmlLine
                        )));


            var paramElement1 = SyntaxFactory
                .XmlParamElement("logger", SyntaxFactory
                    .XmlText("The logger for the class."));

            var paramElement2 = SyntaxFactory
                .XmlParamElement("factory", SyntaxFactory
                    .XmlText("The factory for the class."));

            // Declare the returns XML element
            var seeXmlElement = SyntaxFactory
                .XmlSeeElement(SyntaxFactory
                    .TypeCref(SyntaxFactory
                        .ParseTypeName("XmlDocGen.User")));

            // Declare the returns XML element
            var returnsXmlElement = SyntaxFactory
                .XmlReturnsElement(SyntaxFactory
                    .XmlText("The list of users."));

            // Declare the returns XML element
            var exceptionXmlElement = SyntaxFactory
                .XmlExceptionElement(SyntaxFactory
                        .TypeCref(SyntaxFactory
                            .ParseTypeName("System.Exception")),
                    SyntaxFactory
                        .XmlText("The message of the exception goes here"));

            // Declare new line node 
            var newLineXmlNode = SyntaxFactory
                .XmlText(indentedNewXmlLine);

            // This is the trivia syntax for the entire doc
            var docCommentTriviaSyntax = SyntaxFactory
                .DocumentationComment(
                    summaryXmlElement,
                    newLineXmlNode,
                    seeXmlElement,
                    newLineXmlNode,
                    paramElement1,
                    newLineXmlNode,
                    paramElement2,
                    newLineXmlNode,
                    exceptionXmlElement,
                    newLineXmlNode,
                    returnsXmlElement);

            var newContentTrivia = SyntaxFactory.Trivia(docCommentTriviaSyntax);
            var endOfLineTrivia = SyntaxFactory.EndOfLine(Environment.NewLine);

            var newLeadingTrivia = leadingTrivia
                .Add(newContentTrivia)
                .Add(endOfLineTrivia)
                .Add(indentationTrivia);

            var documentedClassDeclaration = memberDeclarationSyntax
                .WithLeadingTrivia(newLeadingTrivia);

            return documentedClassDeclaration;
        }

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
