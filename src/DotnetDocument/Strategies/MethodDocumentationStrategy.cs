using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Strategies
{
    [Strategy(nameof(SyntaxKind.MethodDeclaration))]
    public class MethodDocumentationStrategy : DocumentationStrategyBase<MethodDeclarationSyntax>
    {
        private readonly ILogger<MethodDocumentationStrategy> _logger;
        private readonly IFormatter _formatter;
        private readonly DeclarationDocOptions _options;

        public MethodDocumentationStrategy(ILogger<MethodDocumentationStrategy> logger,
            IFormatter formatter, IOptions<DotnetDocumentOptions> options) =>
            (_logger, _formatter, _options) = (logger, formatter, options.Value.Method);

        protected override SyntaxKind GetKind() => SyntaxKind.MethodDeclaration;

        public override MethodDeclarationSyntax Apply(MethodDeclarationSyntax node)
        {
            // Get the doc builder for this node
            var builder = GetDocumentationBuilder()
                .For(node);

            // Extract method name
            var methodName = node.Identifier.Text;

            // Extract return type
            var returnType = node.ReturnType.ToString();

            if (returnType is not "void")
            {
                // Extract the last return statement which returns a variable
                // and humanize the name of the variable which will be used as
                // returns descriptions. Empty otherwise.
                var returns = SyntaxUtils
                    .ExtractReturnStatements(node.Body)
                    .Select(r => _formatter
                        .FormatName(_options.Returns.Template, ("{{name}}", r)))
                    .LastOrDefault();

                builder.WithReturns(returns ?? string.Empty);
            }

            // Extract type params and generate a description
            var typeParams = SyntaxUtils
                .ExtractTypeParams(node.TypeParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.TypeParameters.Template, ("{{name}}", p))));

            // Extract params and generate a description
            var @params = SyntaxUtils
                .ExtractParams(node.ParameterList)
                .Select(p => (p, _formatter
                    .FormatName(_options.Parameters.Template, ("{{name}}", p))));

            // Format the summary for this method
            var summary = _formatter.FormatMethod(methodName, returnType, @params.Select(p => p.p));

            builder.WithSummary(summary);

            // Check if single lines comments present in the body block
            // need to be included in the summary of the method 
            if (_options.Summary.IncludeComments)
            {
                var blockComments = SyntaxUtils.ExtractBlockComments(node.Body);

                builder.WithSummary(blockComments);
            }

            if (_options.Exceptions.Enable)
            {
                // Check if constructor has a block body {...}
                if (node.Body is not null)
                {
                    // Extract exceptions from body
                    var extractedExceptions = SyntaxUtils.ExtractThrownExceptions(node.Body).ToList();

                    // Sort them
                    extractedExceptions.Sort((p, n) => string.CompareOrdinal(p.type, n.type));
                    extractedExceptions.Sort((p, n) => string.CompareOrdinal(p.message, n.message));

                    builder.WithExceptions(extractedExceptions);
                }

                // Check if constructor has an expression body => {...}
                if (node.ExpressionBody is not null)
                {
                    // TODO: Extract exceptions in lambda
                }
            }

            return builder
                .WithTypeParams(typeParams)
                .WithParams(@params)
                .Build();
        }
    }
}
