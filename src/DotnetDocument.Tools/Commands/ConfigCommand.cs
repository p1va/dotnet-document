using DotnetDocument.Configuration;
using DotnetDocument.Tools.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetDocument.Tools.Commands
{
    public class ConfigCommand : ICommand<ConfigCommandArgs>
    {
        private readonly ILogger<ConfigCommand> _logger;
        private readonly DocumentationOptions _currentOptions;

        public ConfigCommand(ILogger<ConfigCommand> logger, DocumentationOptions options) =>
            (_logger, _currentOptions) = (logger, options);

        public ExitCode Run(ConfigCommandArgs args) =>
            args.IsDefaultConfig
                ? HandleDefaultConfig()
                : HandlCurrentConfig();

        private ExitCode HandlCurrentConfig()
        {
            // Serialize to YAML the current config
            var yamlConfig = Yaml.Serialize(_currentOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return 0
            return ExitCode.Success;
        }

        private ExitCode HandleDefaultConfig()
        {
            // Create a default config instance
            var defaultOptions = new DocumentationOptions();

            // Get assembly version
            defaultOptions.Version = typeof(Program).Assembly.GetName().Version?.ToString();

            // Serialize to YAML
            var yamlConfig = Yaml.Serialize(defaultOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return 0
            return ExitCode.Success;
        }
    }
}
