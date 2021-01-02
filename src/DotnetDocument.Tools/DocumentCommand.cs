using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotnetDocument.Configuration;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Tools
{
    public class MemberDocumentationStatus
    {
        public string FilePath { get; set; }
        public string Identifier { get; set; }
        public SyntaxKind Kind { get; set; }
        public bool IsDocumented { get; set; }
        public SyntaxNode DocumentedNode { get; set; }
    }

    public class DocumentCommand : ICommand
    {
        private readonly ILogger<DocumentCommand> _logger;
        private readonly IDocumentationStrategy.ServiceResolver _resolver;
        private readonly DotnetDocumentOptions _dotnetDocumentSettings;
        private readonly DocumentationSyntaxWalker _walker;

        public DocumentCommand(ILogger<DocumentCommand> logger, IDocumentationStrategy.ServiceResolver resolver,
            DocumentationSyntaxWalker walker, IOptions<DotnetDocumentOptions> appSettings) =>
            (_logger, _resolver, _walker, _dotnetDocumentSettings) = (logger, resolver, walker, appSettings.Value);

        private IEnumerable<MemberDocumentationStatus> GetFileDocumentationStatus(string filePath)
        {
            string fileContent;

            try
            {
                // Read file content
                fileContent = File.ReadAllText(filePath);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("File {Path} was not found", filePath);

                throw;
            }

            // Declare a new CSharp syntax tree
            var tree = CSharpSyntaxTree.ParseText(fileContent,
                new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

            // Get the compilation unit root
            var root = tree.GetCompilationUnitRoot();

            _walker.Clean();
            _walker.Visit(root);

            foreach (var node in _walker.NodesWithXmlDoc)
            {
                yield return new MemberDocumentationStatus
                {
                    FilePath = filePath,
                    Kind = node.Kind(),
                    IsDocumented = true,
                    Identifier = SyntaxUtils.FindMemberIdentifier(node),
                };
            }

            foreach (var node in _walker.NodesWithoutXmlDoc)
            {
                var nodeWithDoc = _resolver(node.Kind())?.Apply(node);

                yield return new MemberDocumentationStatus
                {
                    FilePath = filePath,
                    Kind = node.Kind(),
                    IsDocumented = false,
                    Identifier = SyntaxUtils.FindMemberIdentifier(node),
                    DocumentedNode = nodeWithDoc
                };
            }
        }

        private IList<MemberDocumentationStatus> GetFilesDocumentationStatus(IEnumerable<string> filePaths) =>
            filePaths
                .SelectMany(GetFileDocumentationStatus)
                .ToList();

        private ExitCode HandleDryRun(CommandArgs args)
        {
            // Retrieve the status of all the members of all the files
            var memberDocStatusList = GetFilesDocumentationStatus(args.Include);

            foreach (var member in memberDocStatusList.Where(m => m.IsDocumented is not true))
            {
                _logger.LogInformation("{File} {MemberType} {MemberName} is not documented",
                    member.FilePath, member.Kind, member.Identifier);
            }

            // In case of members without doc return non zero
            return memberDocStatusList.Any(m => m.IsDocumented is not true)
                ? ExitCode.UndocumentedMembers
                : ExitCode.Success;
        }

        private ExitCode HandleDocument(CommandArgs args)
        {
            // Check and apply changes
            foreach (var file in args.Include)
            {
                // Declare a new CSharp syntax tree
                var tree = CSharpSyntaxTree.ParseText(file,
                    new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

                // Get the compilation unit root
                var root = tree.GetCompilationUnitRoot();

                // Replace the 
                var changedSyntaxTree = root.ReplaceNodes(_walker.NodesWithoutXmlDoc,
                    (node, syntaxNode) => _resolver(syntaxNode.Kind())?.Apply(syntaxNode) ?? syntaxNode);

                File.WriteAllText($"{file.Replace(".cs", "-")}{Guid.NewGuid()}.cs",
                    changedSyntaxTree.ToFullString());
            }

            // Return 0 otherwise
            return ExitCode.Success;
        }

        public ExitCode Run(CommandArgs args)
        {
            try
            {
                // Check if is just a dry run or apply command
                return args.IsDryRun
                    ? HandleDryRun(args)
                    : HandleDocument(args);
            }
            catch (FileNotFoundException)
            {
                return ExitCode.FileNotFound;
            }
        }
    }
}
