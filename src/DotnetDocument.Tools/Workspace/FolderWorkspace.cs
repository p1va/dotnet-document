using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            .EnumerateFiles(_workspacePath, "*.cs", SearchOption.AllDirectories)
            .Where(f =>
                // This is a little bit of an hack
                // Exclude every *.cs file that could be present in bin and obj folders
                // Exclude all the paths that contains /bin/ or /obj/
                !f.Contains("/bin/") && !f.Contains("/obj/") &&
                // Exclude all the paths that contains \bin\ or \obj\
                !f.Contains("\\bin\\") && !f.Contains("\\obj\\"));
    }
}
