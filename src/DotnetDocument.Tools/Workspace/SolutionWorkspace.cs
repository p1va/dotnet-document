using System.Collections.Generic;
using System.IO;
using DotnetDocument.Utils;

namespace DotnetDocument.Tools.Workspace
{
    public class SolutionWorkspace : FolderWorkspace, IWorkspace
    {
        private readonly string _slnPath;

        public SolutionWorkspace(string slnPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(OnlyWhen.NotNull(Path.GetDirectoryName(slnPath)), include, exclude)
        {
            _slnPath = slnPath;
        }

        public new WorkspaceInfo Load() => new(_slnPath, WorkspaceKind.Solution, LoadFiles());
    }
}
