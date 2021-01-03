using System;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace DotnetDocument.Tests.Strategies
{
    [Collection("Constructor documentation strategy")]
    public class ConstructorDocumentationStrategyTests : DocumentationStrategyTestsBase
    {
        [Theory(DisplayName = "Should document")]
        [InlineData(TestCode.WithoutDoc.BlockCtor, TestCode.WithDoc.BlockCtor)]
        public void ShouldDocument(string uncommentedCode, string expectedCommentedCode)
        {
            // Arrange
            var options = Options.Create(new DotnetDocumentOptions
            {
                Constructor = new DeclarationDocOptions(
                    Enable: true,
                    Summary: new SummaryDocumentationOptions(
                        Template: "Creates a new instance of the {{name}} class."),
                    Parameters: new ParametersDocumentationOptions(
                        Enable: true,
                        Template: "The {{name}}."),
                    TypeParameters: new TypeParametersDocumentationOptions(
                        Enable: true,
                        Template: "The {{name}}."),
                    Exceptions: new ExceptionsDocumentationOptions(
                        Enable: true),
                    Returns: new ReturnsDocumentationOptions(
                        Enable: true,
                        Template: "The {{name}}"))
            });

            var ctorDeclarationSyntax = SyntaxUtils.Parse<ConstructorDeclarationSyntax>(uncommentedCode);

            var strategy = new ConstructorDocumentationStrategy(
                NullLogger<ConstructorDocumentationStrategy>.Instance,
                new HumanizeFormatter(),
                options);

            // Act
            var documentedSyntax = strategy.Apply(ctorDeclarationSyntax);

            // Assert
            documentedSyntax.ToFullString().Trim().ShouldBe(expectedCommentedCode.Trim());
        }
    }
}
