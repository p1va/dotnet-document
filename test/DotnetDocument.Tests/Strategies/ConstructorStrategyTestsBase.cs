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
            var options = new CtorDocumentationOptions
            {
                Enabled = true,
                Required = true,
                Summary = new SummaryDocumentationOptions
                {
                    Template = "Creates a new instance of the {{name}} class."
                },
                Parameters = new ParamsDocumentationOptions
                {
                    Enabled = true,
                    Template = "The {{name}}."
                },
                Exceptions = new ExceptionDocumentationOptions()
                {
                    Enabled = true
                }
            };

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
