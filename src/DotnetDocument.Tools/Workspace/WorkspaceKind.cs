namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The workspace kind enum
    /// </summary>
    public enum WorkspaceKind
    {
        /// <summary>
        /// The invalid workspace kind
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The file workspace kind
        /// </summary>
        File = 1,

        /// <summary>
        /// The folder workspace kind
        /// </summary>
        Folder = 2,

        /// <summary>
        /// The project workspace kind
        /// </summary>
        Project = 3,

        /// <summary>
        /// The solution workspace kind
        /// </summary>
        Solution = 4
    }
}
