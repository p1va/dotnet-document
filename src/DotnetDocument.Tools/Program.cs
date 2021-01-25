using System;
using System.Diagnostics;
using CommandLine;
using DotnetDocument.Tools.Commands;
using DotnetDocument.Tools.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DotnetDocument.Tools
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Declare the logger configuration
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}",
                    theme: SystemConsoleTheme.Literate)
                .MinimumLevel.Is(LogEventLevel.Information)
                .CreateLogger();

            // Declare a new service collection
            var services = new ServiceCollection();

            // Configure services collection
            services.AddDotnetDocument();

            // Parse the args
            var cliArgs = Parser.Default
                .ParseArguments<ApplyCommandArgs, ConfigCommandArgs>(args);

            // Configure options depending on the args
            cliArgs
                .WithParsed((ApplyCommandArgs o) => services.ConfigureOptions(IdentifyConfigFileToUse(o.ConfigFile)))
                .WithParsed((ConfigCommandArgs o) => services.ConfigureOptions(IdentifyConfigFileToUse(o.ConfigFile)))
                .WithNotParsed(errors => services.ConfigureOptions());

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the logger from the service provider
            var logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("dotnet-document");

            try
            {
                // Parse command line args
                return cliArgs
                    .MapResult((ApplyCommandArgs opts) => HandleCommand(opts, serviceProvider),
                        (ConfigCommandArgs opts) => HandleCommand(opts, serviceProvider),
                        errors => (int)ExitCode.ArgsParsingError);
            }
            catch (Exception e)
            {
                Log.Logger.Error($"{e.Demystify()}");

                return (int)ExitCode.GeneralError;
            }
        }

        private static int HandleCommand<TArgs>(TArgs opts, IServiceProvider serviceProvider) =>
            (int)(serviceProvider.GetService<ICommand<TArgs>>()?.Run(opts) ?? ExitCode.GeneralError);

        private static string? IdentifyConfigFileToUse(string? argsConfigFilePath)
        {
            string? configFilePath = null;

            // If the config file path is provided via -c use it
            if (!string.IsNullOrWhiteSpace(argsConfigFilePath))
            {
                Log.Logger.Debug("Using config file provided via CLI: {Path}", argsConfigFilePath);

                // Take the config file path from -c
                configFilePath = argsConfigFilePath;
            }
            // If no -c was provided fall back to the env var called DOTNET_DOCUMENT_CONFIG_FILE
            else if (EnvVar.TryGetConfigFile(out var envVarConfigFilePath))
            {
                Log.Logger.Debug("Using config file provided via env: {Path}", envVarConfigFilePath);

                configFilePath = envVarConfigFilePath;
            }

            return configFilePath;
        }
    }
}
