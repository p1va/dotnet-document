using System.Collections.Generic;
using System.IO;

namespace DotnetDocument.Tools.Workspace
{
    public class ProjectWorkspace : FolderWorkspace, IWorkspace
    {
        private readonly string _csprojPath;

        public ProjectWorkspace(string csprojPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(Path.GetDirectoryName(csprojPath), include, exclude)
        {
            _csprojPath = csprojPath;
        }

        public new WorkspaceInfo Load() => new()
        {
            Path = _csprojPath,
            Kind = WorkspaceKind.Project,
            Files = LoadFiles()
        };
    }
}
