using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    /// <summary>
    /// The syntax utils class
    /// </summary>
    public static class SyntaxUtils
    {
        /// <summary>
        /// Gets the indentation trivia using the specified node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The syntax trivia</returns>
        public static SyntaxTrivia? GetIndentationTrivia(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();

            try
            {
                if (leadingTrivia.Count > 0)
                {
                    // Retrieve the last trivia item (usually indentation)
                    var lastTrivia = leadingTrivia.Last();

                    // Check if the last trivia is an end-of-line trivia
                    if (lastTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        return null;

                    }
                    else
                    {
                        // If it's not an end-of-line trivia, return it directly
                        return lastTrivia;
                    }
                }

                // No leading trivia found
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(node.ToFullString() + Environment.NewLine + e);

                // It should be an empty trivia, but I can't figure it out so I just ignore it later
                return null;
            }
        }

        // Not used
        /// <summary>
        /// Gets the indentation element using the specified node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The indentation trivia</returns>
        public static SyntaxTrivia GetIndentationElement(SyntaxNode node)
        {
            var leadingTrivia = node
                .GetLeadingTrivia();

            var indentationTrivia = leadingTrivia
                .LastOrDefault();

            return indentationTrivia;
        }

        /// <summary>
        /// Gets the xml documents using the specified node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>A list of documentation comment trivia syntax</returns>
        public static IList<DocumentationCommentTriviaSyntax> GetXmlDocuments(SyntaxNode node) => node
            .GetLeadingTrivia()
            .Select(s => s.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .ToList();

        /// <summary>
        /// Describes whether is documented
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The bool</returns>
        public static bool IsDocumented(SyntaxNode node) =>
            GetXmlDocuments(node).Any();

        /// <summary>
        /// Finds the member identifier using the specified node
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The descendant identifier</returns>
        public static string FindMemberIdentifier(SyntaxNode node)
        {
            var directNodeIdentifier = node
                .ChildTokens()
                .FirstOrDefault(t => t.Kind() == SyntaxKind.IdentifierToken)
                .Text;

            if (!string.IsNullOrWhiteSpace(directNodeIdentifier)) return directNodeIdentifier;

            var descendantIdentifier = node
                .DescendantTokens()
                .LastOrDefault(t => t.Kind() == SyntaxKind.IdentifierToken)
                .Text;

            return descendantIdentifier;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="constructorDeclarationSyntax">The constructor declaration syntax.</param>
        /// <returns></returns>
        public static string ExtractClassName(ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            if (constructorDeclarationSyntax.Parent is ClassDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.TypeParameterList != null)
            {
                var typeParams = string.Join(",",
                    classDeclarationSyntax.TypeParameterList.Parameters.Select(x => x.Identifier.Text));

                return $"{constructorDeclarationSyntax.Identifier.Text}{{{typeParams}}}";
            }

            return constructorDeclarationSyntax.Identifier.Text;
        }

        /// <summary>
        /// Extracts the base types using the specified class declaration syntax
        /// </summary>
        /// <param name="classDeclarationSyntax">The class declaration syntax</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractBaseTypes(ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.BaseList is not null)
                return classDeclarationSyntax.BaseList.Types
                    .Select(t => t.Type.ToString().Replace("<", "{").Replace(">", "}").Trim());

            return new List<string>();
        }

        /// <summary>
        /// Extracts the base types using the specified interface declaration syntax
        /// </summary>
        /// <param name="interfaceDeclarationSyntax">The interface declaration syntax</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractBaseTypes(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
        {
            if (interfaceDeclarationSyntax.BaseList is not null)
                return interfaceDeclarationSyntax.BaseList.Types
                    .Select(t => t.Type.ToString().Replace("<", "{").Replace(">", "}").Trim());

            return new List<string>();
        }

        /// <summary>
        /// Extracts the exception from expression using the specified throw expression
        /// </summary>
        /// <param name="throwExpression">The throw expression</param>
        /// <returns>The string type string message</returns>
        public static (string type, string message) ExtractExceptionFromExpression(ExpressionSyntax throwExpression)
        {
            var type = string.Empty;
            var message = string.Empty;

            // Check if the throw statement is object creation
            // For example: throw new Exception("Something went wrong");
            if (throwExpression is not ObjectCreationExpressionSyntax exceptionInitSyntax)
                // TODO: Find a way to identify the type of the throw exception
                return (type, message);

            // Get the type of the exception
            // TODO: identify full type name. For example System.Exception
            type = exceptionInitSyntax.Type.ToFullString();

            if (string.IsNullOrWhiteSpace(type)) return (type, message);

            // Try to extract the parameters of the exception ctor
            var exceptionArgExpressions = exceptionInitSyntax.ArgumentList?.Arguments
                .Select(a => a.Expression);

            if (exceptionArgExpressions is null) return (type, message);

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
                    message = partialMessage;
                else
                    message = $"{message} {partialMessage}";
            }

            return (type, message);
        }

        /// <summary>
        /// Extracts the thrown exceptions using the specified body
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>An enumerable of string type and string message</returns>
        public static IEnumerable<(string type, string message)> ExtractThrownExceptions(BlockSyntax body)
        {
            // Get all of the descendant nodes of each body statement
            var descendantNodes = body.Statements
                .SelectMany(s => s.DescendantNodesAndSelf())
                .ToList();

            // Find expressions of throw statements in block body
            var throwStatements = descendantNodes
                .OfType<ThrowStatementSyntax>()
                .Where(e => e.Expression is not null)
                .Select(e => e.Expression!);

            // Find throw expressions which are not root level throw statements
            var throwExpressions = descendantNodes
                .OfType<ThrowExpressionSyntax>()
                .Select(e => e.Expression);

            // Iterate over all of the expressions
            foreach (var throwExpression in throwStatements.Concat(throwExpressions))
            {
                var exception = ExtractExceptionFromExpression(throwExpression);

                if (!string.IsNullOrWhiteSpace(exception.type)) yield return exception;
            }
        }

        /// <summary>
        /// Extracts the params using the specified params
        /// </summary>
        /// <param name="@params">The params</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractParams(ParameterListSyntax? @params) => @params?
                .Parameters
                .Select(p => p.Identifier.Text)
            ?? new List<string>();

        /// <summary>
        /// Extracts the type params using the specified type params
        /// </summary>
        /// <param name="typeParams">The type params</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractTypeParams(TypeParameterListSyntax? typeParams) => typeParams?
                .Parameters
                .Select(p => p.Identifier.Text)
            ?? new List<string>();

        /// <summary>
        /// Extracts the block comments using the specified body
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractBlockComments(BlockSyntax? body) => body?
                .DescendantTrivia()
                .Where(trivia => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .Select(commentTrivia => commentTrivia
                    .ToFullString()
                    .Replace("//", string.Empty)
                    .Trim())
            ?? new List<string>();

        /// <summary>
        /// Extracts the return statements using the specified body
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>An enumerable of string</returns>
        public static IEnumerable<string> ExtractReturnStatements(BlockSyntax body)
        {
            if (body is null) yield break;

            foreach (var returnStatement in body.Statements.OfType<ReturnStatementSyntax>())
                if (returnStatement.Expression is IdentifierNameSyntax identifierName)
                    yield return identifierName.Identifier.Text;
        }

        /// <summary>
        /// Parses the code text
        /// </summary>
        /// <typeparam name="TSyntaxNode">The syntax node</typeparam>
        /// <param name="codeText">The code text</param>
        /// <returns>The syntax node</returns>
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
