using System.Collections.Generic;
using System.IO;

namespace DotnetDocument.Tools.Workspace
{
    public class SolutionWorkspace : FolderWorkspace, IWorkspace
    {
        private readonly string _slnPath;

        public SolutionWorkspace(string slnPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(Path.GetDirectoryName(slnPath), include, exclude)
        {
            _slnPath = slnPath;
        }

        public new WorkspaceInfo Load() => new()
        {
            Path = _slnPath,
            Kind = WorkspaceKind.Solution,
            Files = LoadFiles()
        };
    }
}
