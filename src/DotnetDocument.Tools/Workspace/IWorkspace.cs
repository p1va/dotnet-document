namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The workspace interface
    /// </summary>
    public interface IWorkspace
    {
        /// <summary>
        /// Loads this instance
        /// </summary>
        /// <returns>The workspace info</returns>
        WorkspaceInfo Load();
    }
}
