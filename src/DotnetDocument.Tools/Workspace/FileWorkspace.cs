using System.Collections.Generic;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The file workspace class
    /// </summary>
    /// <seealso cref="IWorkspace" />
    public class FileWorkspace : IWorkspace
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
        /// Initializes a new instance of the <see cref="FileWorkspace" /> class
        /// </summary>
        /// <param name="workspacePath">The workspace path</param>
        /// <param name="include">The include</param>
        /// <param name="exclude">The exclude</param>
        public FileWorkspace(string workspacePath, IEnumerable<string> include, IEnumerable<string> exclude) =>
            (_workspacePath, _includePaths, _excludePaths) =
            (workspacePath, include, exclude);

        /// <summary>
        /// Loads this instance
        /// </summary>
        /// <returns>The workspace info</returns>
        public WorkspaceInfo Load() => new(_workspacePath, WorkspaceKind.File, new[]
        {
            _workspacePath
        });
    }
}
