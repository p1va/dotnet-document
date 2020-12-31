using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Tools
{
    class Program
    {
        public delegate IDocumentationStrategy ServiceResolver(SyntaxKind key);

        public static IDocumentationStrategy Resolve(SyntaxKind kind, IServiceProvider provider)
        {
            var logger = provider
                .GetService<ILoggerFactory>()
                .CreateLogger<ServiceResolver>();

            logger.LogTrace("Resolving documentation strategy for {Kind}", kind);

            var documentationStrategy = provider
                .GetServices<IDocumentationStrategy>()
                .FirstOrDefault(o => o.GetKind() == kind);

            if (documentationStrategy is null)
            {
                logger.LogWarning("No documentation strategy resolved for {Kind}", kind);
            }
            else
            {
                logger.LogTrace("Resolved {DocumentationStrategy} for {Kind}", documentationStrategy?.GetType(), kind);
            }

            return documentationStrategy;
        }

        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(c =>
                {
                    c.SetMinimumLevel(LogLevel.Trace);
                    c.AddConsole();
                })
                .AddOptions()
                .AddScoped<IDocumentationStrategy, ClassDeclarationDocumentationStrategy>()
                .AddScoped<IDocumentationStrategy, ConstructorDeclarationDocumentationStrategy>()
                .AddScoped<IDocumentationStrategy, MethodDeclarationDocumentationStrategy>()
                .AddScoped<ServiceResolver>(provider => kind => Resolve(kind, provider))
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("dotnet-format");

            //do the actual work here
            var resolver = serviceProvider.GetService<ServiceResolver>();

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
                logger.LogDebug("no doc: {NodeName}", node.GetType().FullName);
            }

            foreach (var node in walker.NodesWithXmlDoc)
            {
                logger.LogDebug("has doc: {NodeName}", node.GetType().FullName);
            }

            var changedSyntaxTree = root.ReplaceNodes(walker.NodesWithoutXmlDoc,
                (node, syntaxNode) => resolver(syntaxNode.Kind())?.Apply(syntaxNode));

            File.WriteAllText($"{args[0].Replace(".cs", "-")}{Guid.NewGuid()}.cs", changedSyntaxTree.ToFullString());

            logger.LogDebug("completed");
        }
    }
}
