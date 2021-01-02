using DotnetDocument.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetDocument.Tools.Commands
{
    public class ConfigCommand : ICommand<ConfigCommandArgs>
    {
        private readonly ILogger<ConfigCommand> _logger;
        private readonly DotnetDocumentOptions _currentOptions;

        public ConfigCommand(ILogger<ConfigCommand> logger, IOptions<DotnetDocumentOptions> appSettings) =>
            (_logger, _currentOptions) = (logger, appSettings.Value);

        public ExitCode Run(ConfigCommandArgs args) =>
            args.IsDefaultConfig
                ? HandleDefaultConfig()
                : HandlCurrentConfig();

        private ExitCode HandlCurrentConfig()
        {
            // Serialize to YAML the current config
            var yamlConfig = Serialize(_currentOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return 0
            return ExitCode.Success;
        }

        private ExitCode HandleDefaultConfig()
        {
            // Create a default config instance
            var defaultOptions = new DotnetDocumentOptions();

            // Serialize to YAML
            var yamlConfig = Serialize(defaultOptions);

            // Print the config
            _logger.LogInformation(yamlConfig);

            // Return 0
            return ExitCode.Success;
        }

        private static string Serialize(DotnetDocumentOptions config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .DisableAliases()
                .Build();

            return serializer.Serialize(config);
        }
    }
}
