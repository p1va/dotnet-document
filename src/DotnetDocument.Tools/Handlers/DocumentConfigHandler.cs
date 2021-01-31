using DotnetDocument.Configuration;
using DotnetDocument.Tools.Config;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Tools.Handlers
{
    /// <summary>
    /// The document config handler class
    /// </summary>
    /// <seealso cref="IDocumentConfigHandler" />
    public class DocumentConfigHandler : IDocumentConfigHandler
    {
        /// <summary>
        /// The current options
        /// </summary>
        private readonly DocumentationOptions _currentOptions;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DocumentConfigHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentConfigHandler" /> class
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="options">The options</param>
        public DocumentConfigHandler(ILogger<DocumentConfigHandler> logger, DocumentationOptions options) =>
            (_logger, _currentOptions) = (logger, options);

        /// <summary>
        /// Prints the current config
        /// </summary>
        /// <returns>The result</returns>
        public Result PrintCurrentConfig()
        {
            // Serialize to YAML the current config
            var yamlConfig = Yaml.Serialize(_currentOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return success
            return Result.Success;
        }

        /// <summary>
        /// Prints the default config
        /// </summary>
        /// <returns>The result</returns>
        public Result PrintDefaultConfig()
        {
            // Create a default config instance
            var defaultOptions = new DocumentationOptions();

            // Get assembly version
            defaultOptions.Version = typeof(Program).Assembly.GetName().Version?.ToString();

            // Serialize to YAML
            var yamlConfig = Yaml.Serialize(defaultOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return success
            return Result.Success;
        }
    }
}
