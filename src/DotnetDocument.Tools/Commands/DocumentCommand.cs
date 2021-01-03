using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Tools.Commands
{
    public class DocumentCommand : ICommand<DocumentCommandArgs>
    {
        private readonly ILogger<DocumentCommand> _logger;
        private readonly IServiceResolver<IDocumentationStrategy> _serviceResolver;
        private readonly DotnetDocumentOptions _dotnetDocumentSettings;
        private readonly DocumentationSyntaxWalker _walker;

        public DocumentCommand(ILogger<DocumentCommand> logger,
            IServiceResolver<IDocumentationStrategy> serviceResolver,
            DocumentationSyntaxWalker walker, IOptions<DotnetDocumentOptions> appSettings) =>
            (_logger, _serviceResolver, _walker, _dotnetDocumentSettings) =
            (logger, serviceResolver, walker, appSettings.Value);

        public ExitCode Run(DocumentCommandArgs args)
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
                return ExitCode.FileNotFound;
            }
        }

        private ExitCode HandleDryRun(DocumentCommandArgs args)
        {
            // Retrieve the status of all the members of all the files
            var memberDocStatusList = GetFilesDocumentationStatus(args.Include);

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

        private ExitCode HandleDocument(DocumentCommandArgs args)
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
                    (node, syntaxNode) => _serviceResolver
                        .Resolve(syntaxNode.Kind().ToString())?
                        .Apply(syntaxNode) ?? syntaxNode);

                File.WriteAllText($"{file.Replace(".cs", "-")}{Guid.NewGuid()}.cs",
                    changedSyntaxTree.ToFullString());
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
