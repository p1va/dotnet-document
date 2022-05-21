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
    /// The constructor documentation strategy tests class
    /// </summary>
    /// <seealso cref="DocumentationStrategyTestsBase" />
    [Collection("Constructor documentation strategy")]
    public class ConstructorDocumentationStrategyTests : DocumentationStrategyTestsBase
    {
        /// <summary>
        /// Tests that should document
        /// </summary>
        /// <param name="uncommentedCode">The uncommented code</param>
        /// <param name="expectedCommentedCode">The expected commented code</param>
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
                    Template = "Creates a new instance of the {name} class."
                },
                Parameters = new ParamsDocumentationOptions
                {
                    Enabled = true,
                    Template = "The {name}."
                },
                Exceptions = new ExceptionDocumentationOptions
                {
                    Enabled = true
                }
            };

            var ctorDeclarationSyntax = SyntaxUtils.Parse<ConstructorDeclarationSyntax>(uncommentedCode);

            var strategy = new ConstructorDocumentationStrategy(NullLogger<ConstructorDocumentationStrategy>.Instance,
                new HumanizeFormatter(new DocumentationOptions()),
                options);

            // Act
            var documentationAttempt = strategy.Apply(ctorDeclarationSyntax);

            // Assert
            documentationAttempt.IsChanged.ShouldBe(true);
            documentationAttempt.NodeWithDocs.ToFullString().Trim().ShouldBe(expectedCommentedCode.Trim());
        }
    }
}
