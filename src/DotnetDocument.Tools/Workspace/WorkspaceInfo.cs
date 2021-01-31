using System.Collections.Generic;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The workspace info class
    /// </summary>
    public class WorkspaceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceInfo" /> class
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="kind">The kind</param>
        /// <param name="files">The files</param>
        public WorkspaceInfo(string path, WorkspaceKind kind, IEnumerable<string> files) =>
            (Path, Kind, Files) = (path, kind, files);

        /// <summary>
        /// Gets the value of the path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the value of the kind
        /// </summary>
        public WorkspaceKind Kind { get; }

        /// <summary>
        /// Gets the value of the files
        /// </summary>
        public IEnumerable<string> Files { get; }
    }
}
