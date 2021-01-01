using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Tests.Strategies
{
    public abstract class DocumentationStrategyTestsBase
    {
        protected static TSyntaxNode Parse<TSyntaxNode>(string codeText) where TSyntaxNode : SyntaxNode
        {
            // Declare a new CSharp syntax tree by parsing the program text
            var tree = CSharpSyntaxTree.ParseText(codeText,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            // Find the first syntax node matching the specified type
            return root.Members.First()
                .DescendantNodesAndSelf()
                .OfType<TSyntaxNode>()
                .First();
        }
    }
}
