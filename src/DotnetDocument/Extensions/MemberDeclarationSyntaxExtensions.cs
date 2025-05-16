using System;
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
                var modifierArray = modifier.Split(" ");

                if (node != null)
                {
                    if (modifierArray.Length > 1)
                    {
                        if (node.Modifiers.Any(m => m.Text.Equals(modifierArray[0], StringComparison.Ordinal)) &&
                            node.Modifiers.Any(m => m.Text.Equals(modifierArray[1], StringComparison.Ordinal)) &&
                            !SyntaxUtils.IsDocumented(node))
                        {
                            shouldDocument = true;

                            break;
                        }

                    }
                    else
                    {
                        if (node.Modifiers.All(m => m.Text.Equals(modifier, StringComparison.Ordinal)) &&
                            !SyntaxUtils.IsDocumented(node))
                        {
                            shouldDocument = true;

                            break;
                        }

                        if (node.Modifiers.Any() && node.Modifiers[0].Text.Equals(modifier, StringComparison.Ordinal) &&
                            !SyntaxUtils.IsDocumented(node))
                        {
                            shouldDocument = true;

                            break;
                        }
                    }
                }
            }

            return shouldDocument;
        }
    }
}
