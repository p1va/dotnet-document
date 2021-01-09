using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotnetDocument.Tools.Workspace
{
    public interface IWorkspace
    {
        WorkspaceInfo Load();
    }

    public abstract class WorkspaceBase
    {
    }

    public static class WorkspaceFactory
    {
        public static bool TryFindCsProj(string path, out string csprojFile)
        {
            csprojFile = Directory
                .EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            return csprojFile is not null;
        }

        public static bool TryFindSln(string path, out string slnFile)
        {
            slnFile = Directory
                .EnumerateFiles(path, "*.sln", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            return slnFile is not null;
        }

        public static bool IsCsProj(string path) => path.EndsWith(".csproj");
        public static bool IsSln(string path) => path.EndsWith(".sln");

        public static string GetDefaultTargetPath()
        {
            // TODO: Retrieve current directory
            var currentFolder = Directory.GetCurrentDirectory();

            // Is there any sln?
            if (TryFindSln(currentFolder, out var slnFilePath))
            {
                return slnFilePath;
            }

            // Is there any sln?
            if (TryFindCsProj(currentFolder, out var csprojFile))
            {
                return csprojFile;
            }

            return currentFolder;
        }

        public static IWorkspace Create(string path, IEnumerable<string> include, IEnumerable<string> exclude)
        {
            var isExistingFile = File.Exists(path);
            var isExistingFolder = Directory.Exists(path);

            if (isExistingFile)
            {
                if (IsSln(path))
                {
                    return new SolutionWorkspace(path, include, exclude);
                }

                if (IsCsProj(path))
                {
                    return new ProjectWorkspace(path, include, exclude);
                }

                // Is cs file?
                return new FileWorkspace(path, include, exclude);
            }

            if (isExistingFolder)
            {
                return new FolderWorkspace(path, include, exclude);
            }

            throw new FileNotFoundException($"No file or folder found at path {path}", path);
        }
    }

    public class SolutionWorkspace : WorkspaceBase, IWorkspace
    {
        public SolutionWorkspace(string solutionPath, IEnumerable<string> include, IEnumerable<string> exclude)
        {
        }

        public WorkspaceInfo Load() => throw new NotImplementedException();
    }

    public class ProjectWorkspace : WorkspaceBase, IWorkspace
    {
        public ProjectWorkspace(string projectPath, IEnumerable<string> include, IEnumerable<string> exclude)
        {
        }

        public WorkspaceInfo Load() => throw new NotImplementedException();
    }

    public class FileWorkspace : WorkspaceBase, IWorkspace
    {
        private readonly string _workspacePath;
        private readonly IEnumerable<string> _includePaths;
        private readonly IEnumerable<string> _excludePaths;

        public FileWorkspace(string workspacePath, IEnumerable<string> include, IEnumerable<string> exclude) =>
            (_workspacePath, _includePaths, _excludePaths) =
            (workspacePath, include, exclude);

        public WorkspaceInfo Load() => new()
        {
            Path = _workspacePath,
            Kind = WorkspaceKind.File,
            Files = new[]
            {
                _workspacePath
            }
        };
    }

    public class FolderWorkspace : WorkspaceBase, IWorkspace
    {
        private readonly string _workspacePath;
        private readonly IEnumerable<string> _includePaths;
        private readonly IEnumerable<string> _excludePaths;

        public FolderWorkspace(string workspacePath, IEnumerable<string> include, IEnumerable<string> exclude) =>
            (_workspacePath, _includePaths, _excludePaths) =
            (workspacePath, include, exclude);

        public WorkspaceInfo Load() => new()
        {
            Path = _workspacePath,
            Kind = WorkspaceKind.Folder,
            Files = LoadFiles()
        };

        private IEnumerable<string> LoadFiles() => Directory
            .EnumerateFiles(_workspacePath, "*.cs", SearchOption.AllDirectories);
    }
}
