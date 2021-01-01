using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Strategies
{
    [Collection("Class documentation strategy")]
    public class ClassDocumentationStrategyTests : DocumentationStrategyTestsBase
    {
        [Theory(DisplayName = "Should document")]
        [InlineData(TestCode.WithoutDoc.SimpleClass, TestCode.WithDoc.SimpleClass)]
        [InlineData(TestCode.WithoutDoc.ClassWithInheritance, TestCode.WithDoc.ClassWithInheritance)]
        public void ShouldDocument(string uncommentedCode, string expectedCommentedCode)
        {
            // Arrange
            var options = Options.Create(new DotnetDocumentOptions
            {
                Class = new DeclarationDocOptions(
                    Enable: true,
                    Summary: new SummaryDocumentationOptions(
                        Template: "The {{name}} class",
                        NewLine: true,
                        IncludeInheritance: true,
                        InheritanceTemplate: "Inherits from {{name}}"),
                    Parameters: null,
                    TypeParameters: null)
            });

            var classDeclarationSyntax = Parse<ClassDeclarationSyntax>(uncommentedCode);

            var strategy = new ClassDocumentationStrategy(
                NullLogger<ClassDocumentationStrategy>.Instance,
                new HumanizeFormatter(),
                options);

            // Act
            var documentedSyntax = strategy.Apply(classDeclarationSyntax);

            // Assert
            documentedSyntax.ToFullString().ShouldBe(expectedCommentedCode);
        }
    }
}
