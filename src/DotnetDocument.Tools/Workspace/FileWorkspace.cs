using System.Collections.Generic;

namespace DotnetDocument.Tools.Workspace
{
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
}
