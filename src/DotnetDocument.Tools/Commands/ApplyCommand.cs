using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DotnetDocument.Tools.Commands
{
    public class ApplyCommand : ICommand<ApplyCommandArgs>
    {
        private readonly ILogger<ApplyCommand> _logger;
        private readonly IServiceResolver<IDocumentationStrategy> _serviceResolver;
        private readonly DocumentationOptions _documentationSettings;
        private readonly DocumentationSyntaxWalker _walker;

        public ApplyCommand(ILogger<ApplyCommand> logger,
            IServiceResolver<IDocumentationStrategy> serviceResolver,
            DocumentationSyntaxWalker walker, IOptions<DocumentationOptions> appSettings) =>
            (_logger, _serviceResolver, _walker, _documentationSettings) =
            (logger, serviceResolver, walker, appSettings.Value);

        public ExitCode Run(ApplyCommandArgs args)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Check if is just a dry run or apply command
                var exitCode = args.IsDryRun
                    ? HandleDryRun(args)
                    : HandleDocument(args);

                stopwatch.Stop();

                _logger.LogInformation("Completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

                return exitCode;
            }
            catch (FileNotFoundException)
            {
                _logger.LogInformation("No file or folder found at path '{Path}'", args.Path);

                return ExitCode.FileNotFound;
            }
        }

        private ExitCode HandleDryRun(ApplyCommandArgs args)
        {
            var path = args.Path ?? WorkspaceFactory.GetDefaultTargetPath();
            var includeFiles = args.Include?.Split(" ").ToList() ?? new List<string>();
            var excludeFiles = args.Exclude?.Split(" ").ToList() ?? new List<string>();

            var workspace = WorkspaceFactory.Create(path, includeFiles, excludeFiles);
            var info = workspace.Load();
            var files = info.Files;

            _logger.LogInformation("Applying documentation to {WorkspaceKind} '{WorkspacePath}'",
                info.Kind.ToString().ToLower(), info.Path);

            // Retrieve the status of all the members of all the files
            var memberDocStatusList = GetFilesDocumentationStatus(files);

            foreach (var member in memberDocStatusList.Where(m => m.IsDocumented is not true))
            {
                _logger.LogInformation("  {File} (ln {Line}): {MemberType} '{MemberName}' has no document",
                    member.FilePath, member.StartLine, member.Kind.ToString().Humanize(), member.Identifier);
            }

            // In case of members without doc return non zero
            return memberDocStatusList.Any(m => m.IsDocumented is not true)
                ? ExitCode.UndocumentedMembers
                : ExitCode.Success;
        }

        private ExitCode HandleDocument(ApplyCommandArgs args)
        {
            var path = args.Path ?? WorkspaceFactory.GetDefaultTargetPath();
            var includeFiles = args.Include?.Split(" ").ToList() ?? new List<string>();
            var excludeFiles = args.Exclude?.Split(" ").ToList() ?? new List<string>();

            var workspace = WorkspaceFactory.Create(path, includeFiles, excludeFiles);

            var files = workspace.Load().Files;

            // Retrieve the status of all the members of all the files
            var memberDocStatusList = GetFilesDocumentationStatus(files);

            foreach (var member in memberDocStatusList.Where(m => m.IsDocumented is not true))
            {
                _logger.LogInformation("  {File} (ln {Line}): {MemberType} '{MemberName}' has no document",
                    member.FilePath, member.StartLine, member.Kind.ToString().Humanize(), member.Identifier);
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
                        .Resolve(syntaxNode.Kind().ToString())?
                        .Apply(syntaxNode) ?? syntaxNode);

                // File.WriteAllText($"{file.Replace(".cs", "-")}{Guid.NewGuid()}.cs",
                //     changedSyntaxTree.ToFullString());

                File.WriteAllText(file, changedSyntaxTree.ToFullString());
            }

            // Return 0 otherwise
            return ExitCode.Success;
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
                yield return new MemberDocumentationStatus
                {
                    FilePath = filePath,
                    Kind = node.Kind(),
                    IsDocumented = true,
                    Identifier = SyntaxUtils.FindMemberIdentifier(node),
                    NodeWithoutDocument = node,
                    StartLine = node.GetLocation().GetLineSpan().StartLinePosition.ToString()
                };
            }

            foreach (var node in _walker.NodesWithoutXmlDoc)
            {
                var nodeWithDoc = _serviceResolver
                    .Resolve(node.Kind().ToString())?
                    .Apply(node);

                yield return new MemberDocumentationStatus
                {
                    FilePath = filePath,
                    Kind = node.Kind(),
                    IsDocumented = false,
                    Identifier = SyntaxUtils.FindMemberIdentifier(node),
                    NodeWithoutDocument = node,
                    DocumentedNode = nodeWithDoc,
                    StartLine = node.GetLocation().GetLineSpan().StartLinePosition.ToString()
                };
            }
        }

        private IList<MemberDocumentationStatus> GetFilesDocumentationStatus(IEnumerable<string> filePaths) =>
            filePaths
                .SelectMany(GetFileDocumentationStatus)
                .ToList();
    }
}
