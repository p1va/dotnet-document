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

            try
            {
                var indentationTrivia = leadingTrivia
                    .Last();

                return indentationTrivia;
            }
            catch (Exception e)
            {
                Console.WriteLine(node.ToFullString() + "\n" + e);

                // TODO: Investigate this. It should be an empty trivia
                return SyntaxFactory.Space;
            }
        }

        // Not used
        public static SyntaxTrivia GetIndentationElement(SyntaxNode node)
        {
            var leadingTrivia = node
                .GetLeadingTrivia();

            var indentationTrivia = leadingTrivia
                .LastOrDefault();

            return indentationTrivia;
        }

        public static IList<DocumentationCommentTriviaSyntax> GetXmlDocuments(SyntaxNode node) => node
            .GetLeadingTrivia()
            .Select(s => s.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .ToList();

        public static bool IsDocumented(SyntaxNode node) =>
            GetXmlDocuments(node).Any();

        public static string FindMemberIdentifier(SyntaxNode node)
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
                .LastOrDefault(t => t.Kind() == SyntaxKind.IdentifierToken)
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

        public static (string type, string message) ExtractExceptionFromExpression(ExpressionSyntax throwExpression)
        {
            var type = string.Empty;
            var message = string.Empty;

            // Check if the throw statement is object creation
            // For example: throw new Exception("Something went wrong");
            if (throwExpression is not ObjectCreationExpressionSyntax exceptionInitSyntax)
            {
                // TODO: Find a way to identify the type of the throw exception
                return (type, message);
            }

            // Get the type of the exception
            // TODO: identify full type name. For example System.Exception
            type = exceptionInitSyntax.Type.ToFullString();

            if (string.IsNullOrWhiteSpace(type))
            {
                return (type, message);
            }

            // Try to extract the parameters of the exception ctor
            var exceptionArgExpressions = exceptionInitSyntax.ArgumentList?.Arguments
                .Select(a => a.Expression);

            if (exceptionArgExpressions is null)
            {
                return (type, message);
            }

            foreach (var argExpression in exceptionArgExpressions)
            {
                var partialMessage = string.Empty;

                switch (argExpression)
                {
                    // throw new Exception("This field is wrong");
                    case LiteralExpressionSyntax literal:
                        partialMessage = literal.Token.ValueText;

                        break;

                    // throw new Exception($"This {var} is wrong");
                    case InterpolatedStringExpressionSyntax interpolated:
                        var contents = interpolated.Contents.Select(c => c.ToFullString());
                        partialMessage = string.Join(string.Empty, contents);

                        break;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = partialMessage;
                }
                else
                {
                    message = $"{message} {partialMessage}";
                }
            }

            return (type, message);
        }

        public static IEnumerable<(string type, string message)> ExtractThrownExceptions(BlockSyntax body)
        {
            // Get all of the descendant nodes of each body statement
            var descendantNodes = body.Statements
                .SelectMany(s => s.DescendantNodesAndSelf())
                .ToList();

            // Find expressions of throw statements in block body
            var throwStatements = descendantNodes
                .OfType<ThrowStatementSyntax>()
                .Select(e => e.Expression);

            // Find throw expressions which are not root level throw statements
            var throwExpressions = descendantNodes
                .OfType<ThrowExpressionSyntax>()
                .Select(e => e.Expression);

            // Iterate over all of the expressions
            foreach (var throwExpression in throwStatements.Concat(throwExpressions))
            {
                var exception = ExtractExceptionFromExpression(throwExpression);

                if (!string.IsNullOrWhiteSpace(exception.type))
                {
                    yield return exception;
                }
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

        public static TSyntaxNode Parse<TSyntaxNode>(string codeText) where TSyntaxNode : SyntaxNode
        {
            // Declare a new CSharp syntax tree by parsing the program text
            var tree = CSharpSyntaxTree.ParseText(codeText,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            // Find the first syntax node matching the specified type
            return root.Members.First()
                .DescendantNodesAndSelf()
                .OfType<TSyntaxNode>()
                .First();
        }
    }
}
