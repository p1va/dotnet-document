namespace DotnetDocument.Tools.Handlers
{
    /// <summary>
    /// The document config handler interface
    /// </summary>
    public interface IDocumentConfigHandler
    {
        /// <summary>
        /// Prints the current config
        /// </summary>
        /// <returns>The result</returns>
        Result PrintCurrentConfig();

        /// <summary>
        /// Prints the default config
        /// </summary>
        /// <returns>The result</returns>
        Result PrintDefaultConfig();
    }
}
