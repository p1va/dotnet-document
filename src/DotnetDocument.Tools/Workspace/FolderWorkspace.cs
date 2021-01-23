using System;
using System.Collections.Generic;
using System.IO;

namespace DotnetDocument.Tools.Workspace
{
    public class FolderWorkspace : WorkspaceBase, IWorkspace
    {
        private readonly string _workspacePath;
        private readonly IEnumerable<string> _includePaths;
        private readonly IEnumerable<string> _excludePaths;

        public FolderWorkspace(string workspacePath, IEnumerable<string> include, IEnumerable<string> exclude) =>
            (_workspacePath, _includePaths, _excludePaths) =
            (workspacePath, include, exclude);

        public WorkspaceInfo Load() => new(_workspacePath, WorkspaceKind.Folder, LoadFiles());

        protected IEnumerable<string> LoadFiles() => Directory
            .EnumerateFiles(_workspacePath, "*.cs", SearchOption.AllDirectories);
    }
}
