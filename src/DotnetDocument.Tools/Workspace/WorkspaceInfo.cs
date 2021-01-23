using System.Collections.Generic;

namespace DotnetDocument.Tools.Workspace
{
    public class WorkspaceInfo
    {
        public WorkspaceInfo(string path, WorkspaceKind kind, IEnumerable<string> files) =>
            (Path, Kind, Files) = (path, kind, files);

        public string Path { get; }
        public WorkspaceKind Kind { get; }
        public IEnumerable<string> Files { get; }
    }
}
