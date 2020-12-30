using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Strategies.Abstractions;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Strategies
{
    public class ClassDeclarationDocumentationStrategy : DocumentationStrategyBase<ClassDeclarationSyntax>
    {
        public override SyntaxKind GetKind() => SyntaxKind.ClassDeclaration;

        public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node)
        {
            var className = node.Identifier.Text;

            var summary = new List<string>
            {
                $"The {className.Humanize().ToLower()} class"
            };

            if (node.BaseList is not null)
            {
                var baseTypes = node.BaseList.Types
                    .Select(t => t.Type.ToString());

                foreach (var baseType in baseTypes)
                {
                    summary.Add($"Inherits from {baseType}");
                }
            }

            return GetDocumentationBuilder()
                .For(node)
                .WithSummary(summary.ToArray())
                .Build();
        }
    }
}
