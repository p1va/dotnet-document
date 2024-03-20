using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Strategies
{
    /// <summary>
    /// The class documentation strategy tests class
    /// </summary>
    /// <seealso cref="DocumentationStrategyTestsBase" />
    [Collection("Class documentation strategy")]
    public class ClassDocumentationStrategyTests : DocumentationStrategyTestsBase
    {
        /// <summary>
        /// Tests that should document
        /// </summary>
        /// <param name="uncommentedCode">The uncommented code</param>
        /// <param name="expectedCommentedCode">The expected commented code</param>
        [Theory(DisplayName = "Should document")]
        [InlineData(TestCode.WithoutDoc.SimpleClass, TestCode.WithDoc.SimpleClass)]
        [InlineData(TestCode.WithoutDoc.SimpleClassLeftAlign, TestCode.WithDoc.SimpleClassLeftAlign)]
        [InlineData(TestCode.WithoutDoc.ClassWithInheritance, TestCode.WithDoc.ClassWithInheritance)]
        public void ShouldDocument(string uncommentedCode, string expectedCommentedCode)
        {
            // Arrange
            var classDeclarationSyntax = SyntaxUtils.Parse<ClassDeclarationSyntax>(uncommentedCode);

            var strategy = new ClassDocumentationStrategy(NullLogger<ClassDocumentationStrategy>.Instance,
                new HumanizeFormatter(new DocumentationOptions()),
                new ClassDocumentationOptions());

            // Act
            var documentedSyntax = strategy.Apply(classDeclarationSyntax);

            // Assert
            documentedSyntax.ToFullString().ShouldBe(expectedCommentedCode);
        }
    }
}
