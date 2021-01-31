using System.Collections.Generic;
using System.IO;
using DotnetDocument.Utils;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The project workspace class
    /// </summary>
    /// <seealso cref="FolderWorkspace" />
    /// <seealso cref="IWorkspace" />
    public class ProjectWorkspace : FolderWorkspace, IWorkspace
    {
        /// <summary>
        /// The csproj path
        /// </summary>
        private readonly string _csprojPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectWorkspace" /> class
        /// </summary>
        /// <param name="csprojPath">The csproj path</param>
        /// <param name="include">The include</param>
        /// <param name="exclude">The exclude</param>
        public ProjectWorkspace(string csprojPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(OnlyWhen.NotNull(Path.GetDirectoryName(csprojPath)), include, exclude) =>
            _csprojPath = csprojPath;

        /// <summary>
        /// Loads this instance
        /// </summary>
        /// <returns>The workspace info</returns>
        public new WorkspaceInfo Load() => new(_csprojPath, WorkspaceKind.Project, LoadFiles());
    }
}
