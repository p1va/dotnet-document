namespace DotnetDocument.Tools.Handlers
{
    /// <summary>
    /// The apply document handler interface
    /// </summary>
    public interface IApplyDocumentHandler
    {
        /// <summary>
        /// Applies the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="isDryRun">The is dry run</param>
        /// <returns>The result</returns>
        Result Apply(string? path, bool isDryRun);
    }
}
