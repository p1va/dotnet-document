using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Extensions
{
    public static class MemberDeclarationSyntaxExtensions
    {
        public static bool ShouldDocument(this MemberDeclarationSyntax node, MemberDocumentationOptionsBase options)
        {
            var shouldDocument = false;

            foreach (var modifier in options.AccessModifiers)
            {
                if (node.Modifiers.ToFullString().Contains(modifier) && !SyntaxUtils.IsDocumented(node))
                {
                    shouldDocument = true;
                    break;
                }
            }

            return shouldDocument;
        }
    }
}
