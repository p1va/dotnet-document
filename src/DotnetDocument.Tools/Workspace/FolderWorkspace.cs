using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The folder workspace class
    /// </summary>
    /// <seealso cref="IWorkspace" />
    public class FolderWorkspace : IWorkspace
    {
        /// <summary>
        /// The exclude paths
        /// </summary>
        private readonly IEnumerable<string> _excludePaths;

        /// <summary>
        /// The include paths
        /// </summary>
        private readonly IEnumerable<string> _includePaths;

        /// <summary>
        /// The workspace path
        /// </summary>
        private readonly string _workspacePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWorkspace" /> class
        /// </summary>
        /// <param name="workspacePath">The workspace path</param>
        /// <param name="include">The include</param>
        /// <param name="exclude">The exclude</param>
        public FolderWorkspace(string workspacePath, IEnumerable<string> include, IEnumerable<string> exclude) =>
            (_workspacePath, _includePaths, _excludePaths) =
            (workspacePath, include, exclude);

        /// <summary>
        /// Loads this instance
        /// </summary>
        /// <returns>The workspace info</returns>
        public WorkspaceInfo Load() => new(_workspacePath, WorkspaceKind.Folder, LoadFiles());

        /// <summary>
        /// Loads the files
        /// </summary>
        /// <returns>An enumerable of string</returns>
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
