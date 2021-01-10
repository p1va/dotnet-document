using System.Collections.Generic;

namespace DotnetDocument.Tools.Workspace
{
    public class WorkspaceInfo
    {
        public string Path { get; init; }
        public WorkspaceKind Kind { get; init; }
        public IEnumerable<string> Files { get; init; }
    }
}
