using System.Collections.Generic;
using System.IO;
using DotnetDocument.Utils;

namespace DotnetDocument.Tools.Workspace
{
    public class ProjectWorkspace : FolderWorkspace, IWorkspace
    {
        private readonly string _csprojPath;

        public ProjectWorkspace(string csprojPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(OnlyWhen.NotNull(Path.GetDirectoryName(csprojPath)), include, exclude)
        {
            _csprojPath = csprojPath;
        }

        public new WorkspaceInfo Load() => new(_csprojPath, WorkspaceKind.Project, LoadFiles());
    }
}
