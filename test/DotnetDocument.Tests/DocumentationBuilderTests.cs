using System.Linq;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests
{
    /// <summary>
    /// The documentation builder tests class
    /// </summary>
    public class DocumentationBuilderTests
    {
        /// <summary>
        /// The program text
        /// </summary>
        private const string ProgramText = @"
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";
        /// <summary>
        /// The program text with file scoped namespace
        /// </summary>
        private const string ProgramTextFileScopedNamespace = @"
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello, World!"");
    }
}
";

        /// <summary>
        /// The expected documented method text
        /// </summary>
        private const string ExpectedDocumentedMethodTextBlockScope = @"        /// <summary>
        /// Gets or sets the list of users
        /// Note that this method needs to be awaited
        /// </summary>
        /// <typeparam name=""TEntity"">The type of the returned entity</typeparam>
        /// <param name=""logger"">The logger</param>
        /// <param name=""repository"">The user repository</param>
        /// <exception cref=""System.Exception"">You should provide something</exception>
        /// <exception cref=""System.ArgumentException"">You should provide a valid arg</exception>
        /// <returns>The list of users</returns>
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
";        
        private const string ExpectedDocumentedMethodTextFileScope = @"    /// <summary>
    /// Gets or sets the list of users
    /// Note that this method needs to be awaited
    /// </summary>
    /// <typeparam name=""TEntity"">The type of the returned entity</typeparam>
    /// <param name=""logger"">The logger</param>
    /// <param name=""repository"">The user repository</param>
    /// <exception cref=""System.Exception"">You should provide something</exception>
    /// <exception cref=""System.ArgumentException"">You should provide a valid arg</exception>
    /// <returns>The list of users</returns>
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello, World!"");
    }
";

        /// <summary>
        /// Tests blocked scope namespace
        /// </summary>
        [Fact(DisplayName = "Yes")]
        public void TestBlockedScopeNamespace()
        {
            // Arrange

            // Declare a new CSharp syntax tree by parsing the program text
            var tree = CSharpSyntaxTree.ParseText(ProgramText,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            // Find the first method declaration
            var methodDeclaration = root.Members.First()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First();

            // Assert it's a method declaration
            methodDeclaration.Kind().ShouldBe(SyntaxKind.MethodDeclaration);

            // Act
            var documentedNode = new DocumentationBuilder<MethodDeclarationSyntax>()
                .For(methodDeclaration)
                .WithSummary("Gets or sets the list of users", "Note that this method needs to be awaited")
                .WithTypeParam("TEntity", "The type of the returned entity")
                .WithParam("logger", "The logger")
                .WithParam("repository", "The user repository")
                .WithException("System.Exception", "You should provide something")
                .WithException("System.ArgumentException", "You should provide a valid arg")
                .WithReturns("The list of users")
                .Build();

            // Assert
            documentedNode.ToFullString().ShouldBe(ExpectedDocumentedMethodTextBlockScope);
        }

        /// <summary>
        /// Tests file scoped namespace
        /// </summary>
        [Fact(DisplayName = "Yes")]
        public void TestFileScopeNamespace()
        {
            // Arrange

            // Declare a new CSharp syntax tree by parsing the program text
            var tree = CSharpSyntaxTree.ParseText(ProgramTextFileScopedNamespace,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            // Find the first method declaration
            var methodDeclaration = root.Members.First()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .First();

            // Assert it's a method declaration
            methodDeclaration.Kind().ShouldBe(SyntaxKind.MethodDeclaration);

            // Act
            var documentedNode = new DocumentationBuilder<MethodDeclarationSyntax>()
                .For(methodDeclaration)
                .WithSummary("Gets or sets the list of users", "Note that this method needs to be awaited")
                .WithTypeParam("TEntity", "The type of the returned entity")
                .WithParam("logger", "The logger")
                .WithParam("repository", "The user repository")
                .WithException("System.Exception", "You should provide something")
                .WithException("System.ArgumentException", "You should provide a valid arg")
                .WithReturns("The list of users")
                .Build();

            // Assert
            documentedNode.ToFullString().ShouldBe(ExpectedDocumentedMethodTextFileScope);
        }
    }
}
