using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("dotnet-document");

            var map = new Dictionary<SyntaxKind, IDocumentationStrategy>()
            {
                {SyntaxKind.ClassDeclaration, new ClassDeclarationDocumentationStrategy()},
                {SyntaxKind.ConstructorDeclaration, new ConstructorDeclarationDocumentationStrategy()},
                {SyntaxKind.MethodDeclaration, new MethodDeclarationDocumentationStrategy()}
            };

            // Read file content
            var fileContent = File.ReadAllText(args[0]);

            // Declare a new CSharp syntax tree
            var tree = CSharpSyntaxTree.ParseText(fileContent,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            var walker = new DocumentationSyntaxWalker();

            walker.Visit(root);

            foreach (var node in walker.NodesWithoutXmlDoc)
            {
                Console.WriteLine(node.GetType().FullName ?? string.Empty, Color.Red);
            }

            foreach (var node in walker.NodesWithXmlDoc)
            {
                Console.WriteLine(node.GetType().FullName ?? string.Empty, Color.Green);
            }

            var changedSyntaxTree = root.ReplaceNodes(walker.NodesWithoutXmlDoc,
                (node, syntaxNode) =>
                {
                    if (map.ContainsKey(syntaxNode.Kind()))
                    {
                        return map[syntaxNode.Kind()].Apply(syntaxNode);
                    }

                    return syntaxNode;
                });

            File.WriteAllText($"{args[0].Replace(".cs", "-")}{Guid.NewGuid()}.cs", changedSyntaxTree.ToFullString());
        }
    }
}
