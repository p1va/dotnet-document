using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public static class SyntaxUtils
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

        public static IEnumerable<string> ExtractBaseTypes(ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList is not null)
            {
                return classDeclarationSyntax.BaseList.Types
                    .Select(t => t.Type.ToString().Replace("<", "{").Replace(">", "}").Trim());
            }

            return new List<string>();
        }

        public static IEnumerable<string> ExtractBaseTypes(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
        {
            if (interfaceDeclarationSyntax.BaseList is not null)
            {
                return interfaceDeclarationSyntax.BaseList.Types
                    .Select(t => t.Type.ToString().Replace("<", "{").Replace(">", "}").Trim());
            }

            return new List<string>();
        }

        public static (string type, string message) ExtractException(ThrowStatementSyntax throwStatement)
        {
            var type = string.Empty;
            var message = string.Empty;

            // Check if the throw statement is an object creation like
            // throw new Exception("Something went wrong");
            if (throwStatement.Expression is ObjectCreationExpressionSyntax exceptionCreation)
            {
                // Get the type of the exception
                // TODO: identify full type name like System.Exception
                type = exceptionCreation.Type.ToFullString();

                // Try to extract the parameters of the exception ctor
                // As of now only a simple case like new Exception("Something went wrong"); will work
                // TODO: improve logic to handle more complex scenarios like new Exception(ex, "Something went wrong");
                var exceptionCreationArg = exceptionCreation.ArgumentList?.Arguments.FirstOrDefault();

                if (exceptionCreationArg?.Expression is not null)
                {
                    switch (@exceptionCreationArg.Expression)
                    {
                        case LiteralExpressionSyntax literal:
                            message = literal.Token.ValueText;

                            break;

                        case InterpolatedStringExpressionSyntax interpolated:
                            var contents = interpolated.Contents.Select(c => c.ToFullString());
                            message = string.Join(string.Empty, contents);

                            break;
                    }
                }
            }

            return (type, message);
        }

        public static IEnumerable<(string type, string message)> ExtractThrownExceptions(BlockSyntax body)
        {
            // Find throw statements in block body
            var throwStatements = body.Statements
                .OfType<ThrowStatementSyntax>();

            // Iterate over all of the statements
            foreach (var throwStatement in throwStatements)
            {
                yield return ExtractException(throwStatement);
            }
        }

        public static IEnumerable<string> ExtractParams(ParameterListSyntax @params) => @params?
                .Parameters
                .Select(p => p.Identifier.Text)
            ?? new List<string>();

        public static IEnumerable<string> ExtractTypeParams(TypeParameterListSyntax typeParams) => typeParams?
                .Parameters
                .Select(p => p.Identifier.Text)
            ?? new List<string>();

        public static IEnumerable<string> ExtractBlockComments(BlockSyntax body) => body?
                .DescendantTrivia()
                .Where(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .Select(commentTrivia => commentTrivia
                    .ToFullString()
                    .Replace("//", string.Empty)
                    .Trim())
            ?? new List<string>();

        public static IEnumerable<string> ExtractReturnStatements(BlockSyntax body)
        {
            if (body is null)
            {
                yield break;
            }

            foreach (var returnStatement in body.Statements.OfType<ReturnStatementSyntax>())
            {
                if (returnStatement.Expression is IdentifierNameSyntax identifierName)
                {
                    yield return identifierName.Identifier.Text;
                }
            }
        }
    }
}
