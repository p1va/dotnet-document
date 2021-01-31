namespace DotnetDocument.Strategies.Abstractions
{
    /// <summary>
    /// The service resolver interface
    /// </summary>
    public interface IServiceResolver<out TService>
    {
        /// <summary>
        /// Resolves the key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The service</returns>
        TService? Resolve(string key);
    }
}
