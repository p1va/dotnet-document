using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Workspace;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Tools.Handlers
{
    public class ApplyDocumentHandler : IApplyDocumentHandler
    {
        private readonly ILogger<ApplyDocumentHandler> _logger;
        private readonly IServiceResolver<IDocumentationStrategy> _serviceResolver;
        private readonly DocumentationOptions _documentationSettings;
        private readonly DocumentationSyntaxWalker _walker;

        public ApplyDocumentHandler(ILogger<ApplyDocumentHandler> logger,
            IServiceResolver<IDocumentationStrategy> serviceResolver,
            DocumentationSyntaxWalker walker, IOptions<DocumentationOptions> appSettings) =>
            (_logger, _serviceResolver, _walker, _documentationSettings) =
            (logger, serviceResolver, walker, appSettings.Value);

        public Result Apply(string? path, bool isDryRun)
        {
            // If no path provided use the current path
            path ??= WorkspaceFactory.GetDefaultTargetPath();

            // Create a new workspace from path
            var workspace = WorkspaceFactory.Create(path, new List<string>(), new List<string>());

            // Load all *.cs file of the workspace
            var files = workspace.Load().Files.ToList();

            // Retrieve the status of all the members of all the files
            var memberDocStatusList = GetFilesDocumentationStatus(files);

            // Get the list of undocumented members
            var undocumentedMembers = memberDocStatusList.Where(m => m.IsDocumented is not true).ToList();

            foreach (var member in undocumentedMembers)
            {
                _logger.LogInformation("  {File} (ln {Line}): {MemberType} '{MemberName}' has no document",
                    member.FilePath, member.StartLine, member.Kind.ToString().Humanize(), member.Identifier);
            }

            // If is dry run
            if (isDryRun)
            {
                _logger.LogWarning("Found {MembersCount} members without documentation across {FilesCount} files",
                    undocumentedMembers.Count, undocumentedMembers.Select(m => m.FilePath).Distinct().Count());

                // Don't go ahead with saving.
                // If there are members without doc return `undocumented members`
                return memberDocStatusList.Any(m => m.IsDocumented is not true)
                    ? Result.UndocumentedMembers
                    : Result.Success;
            }

            // Check and apply changes
            foreach (var file in files)
            {
                // Read the file content
                var fileContent = File.ReadAllText(file);

                // Declare a new CSharp syntax tree
                var tree = CSharpSyntaxTree.ParseText(fileContent,
                    new CSharpParseOptions(documentationMode: DocumentationMode.Parse));

                // Get the compilation unit root
                var root = tree.GetCompilationUnitRoot();

                _walker.Clean();
                _walker.Visit(root);

                // Replace the 
                var changedSyntaxTree = root.ReplaceNodes(_walker.NodesWithoutXmlDoc,
                    (node, syntaxNode) => _serviceResolver
                        .Resolve(syntaxNode.Kind().ToString())
                        ?
                        .Apply(syntaxNode) ?? syntaxNode);

                // TODO: Don't write if no changes

                _logger.LogTrace("  Writing changes of {File} to disk", file);

                File.WriteAllText(file, changedSyntaxTree.ToFullString());
            }

            // Return success
            return Result.Success;
        }

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
                yield return new MemberDocumentationStatus(filePath, SyntaxUtils.FindMemberIdentifier(node),
                    node.Kind(), true, null, node,
                    node.GetLocation().GetLineSpan().StartLinePosition.ToString());
            }

            foreach (var node in _walker.NodesWithoutXmlDoc)
            {
                var nodeWithDoc = _serviceResolver
                    .Resolve(node.Kind().ToString())
                    ?
                    .Apply(node);

                yield return new MemberDocumentationStatus(filePath, SyntaxUtils.FindMemberIdentifier(node),
                    node.Kind(), false, node, nodeWithDoc,
                    node.GetLocation().GetLineSpan().StartLinePosition.ToString());
            }
        }

        private IList<MemberDocumentationStatus> GetFilesDocumentationStatus(IEnumerable<string> filePaths) =>
            filePaths
                .SelectMany(GetFileDocumentationStatus)
                .ToList();
    }
}
