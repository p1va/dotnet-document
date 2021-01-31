using System.Collections.Generic;
using System.IO;
using DotnetDocument.Utils;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The solution workspace class
    /// </summary>
    /// <seealso cref="FolderWorkspace" />
    /// <seealso cref="IWorkspace" />
    public class SolutionWorkspace : FolderWorkspace, IWorkspace
    {
        /// <summary>
        /// The sln path
        /// </summary>
        private readonly string _slnPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionWorkspace" /> class
        /// </summary>
        /// <param name="slnPath">The sln path</param>
        /// <param name="include">The include</param>
        /// <param name="exclude">The exclude</param>
        public SolutionWorkspace(string slnPath, IEnumerable<string> include, IEnumerable<string> exclude) :
            base(OnlyWhen.NotNull(Path.GetDirectoryName(slnPath)), include, exclude) =>
            _slnPath = slnPath;

        /// <summary>
        /// Loads this instance
        /// </summary>
        /// <returns>The workspace info</returns>
        public new WorkspaceInfo Load() => new(_slnPath, WorkspaceKind.Solution, LoadFiles());
    }
}
