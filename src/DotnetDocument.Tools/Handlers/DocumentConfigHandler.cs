using DotnetDocument.Configuration;
using DotnetDocument.Tools.Config;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Tools.Handlers
{
    public class DocumentConfigHandler : IDocumentConfigHandler
    {
        private readonly ILogger<DocumentConfigHandler> _logger;
        private readonly DocumentationOptions _currentOptions;

        public DocumentConfigHandler(ILogger<DocumentConfigHandler> logger, DocumentationOptions options) =>
            (_logger, _currentOptions) = (logger, options);

        public Result PrintCurrentConfig()
        {
            // Serialize to YAML the current config
            var yamlConfig = Yaml.Serialize(_currentOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return success
            return Result.Success;
        }

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
