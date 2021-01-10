using System;
using System.IO;
using System.Linq;
using CommandLine;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Commands;
using Microsoft.Extensions.Configuration;
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
                .WriteTo.Console(
                    outputTemplate: "{Message:lj}{NewLine}",
                    theme: ConsoleTheme.None)
                .MinimumLevel.Is(LogEventLevel.Information)
                .CreateLogger();

            // Declare a new service collection
            var services = new ServiceCollection();

            // Configure service
            ConfigureServices(services);

            var parserResult = Parser.Default
                .ParseArguments<ApplyCommandArgs, ConfigCommandArgs>(args);

            parserResult
                .WithParsed((ApplyCommandArgs o) => ConfigureOptions(services, o.ConfigFile))
                .WithParsed((ConfigCommandArgs o) => ConfigureOptions(services, o.ConfigFile))
                .WithNotParsed(errors => ConfigureOptions(services, null));

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the logger
            var logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("dotnet-document");

            // Parse command line args
            return parserResult
                .MapResult(
                    (ApplyCommandArgs opts) => HandleCommand(opts, serviceProvider),
                    (ConfigCommandArgs opts) => HandleCommand(opts, serviceProvider),
                    errors => (int)ExitCode.ArgsParsingError);
        }

        private static int HandleCommand<TArgs>(TArgs opts, IServiceProvider serviceProvider) =>
            (int)serviceProvider.GetService<ICommand<TArgs>>().Run(opts);

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add logging
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            // Add formatter
            services.AddSingleton<IFormatter, HumanizeFormatter>();

            // Add documentation strategies
            services
                .AddTransient<IDocumentationStrategy, ClassDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, InterfaceDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, EnumDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, EnumMemberDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, ConstructorDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, MethodDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, PropertyDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, DefaultDocumentationStrategy>();

            // Add attribute based service resolver
            // This will help resolving the correct documentation strategy
            // For this to work, strategies have to use the [Strategy("key")] attribute
            services
                .AddTransient<
                    IServiceResolver<IDocumentationStrategy>,
                    AttributeServiceResolver<IDocumentationStrategy>>(
                    provider => new AttributeServiceResolver<IDocumentationStrategy>(provider));

            // Add syntax walker
            services.AddTransient(provider =>
            {
                // Retrieve the list of supported SyntaxKinds from the DI
                var supportedDocumentationKinds = provider
                    .GetServices<IDocumentationStrategy>()
                    .SelectMany(s => s.GetSupportedKinds());

                return new DocumentationSyntaxWalker(supportedDocumentationKinds);
            });

            // Add the commands
            services.AddTransient<ICommand<ApplyCommandArgs>, ApplyCommand>();
            services.AddTransient<ICommand<ConfigCommandArgs>, ConfigCommand>();
        }

        private static void ConfigureOptions(IServiceCollection services, string optionsFilePath)
        {
            var documentationOptions = new DocumentationOptions();

            if (!string.IsNullOrWhiteSpace(optionsFilePath))
            {
                //Log.Logger.Information("Loading config file at '{ConfigFilePath}'", optionsFilePath);

                try
                {
                    documentationOptions = Yaml.Deserialize<DocumentationOptions>(optionsFilePath);
                }
                catch (FileNotFoundException e)
                {
                    Log.Logger.Error("No config file found at '{ConfigFilePath}'", optionsFilePath);

                    // TODO: Exit code
                    throw;
                }
            }

            // Convert the documentation options into a list of member specific options
            // For example: Class doc options, Ctor doc options, Property...
            // In this way we can avoid passing the entire options object to each documentation
            // strategy that will only receive the portion of config relevant to them
            documentationOptions.ToList()
                .ForEach(d => services.AddSingleton(d.GetType(), d));

            // Add also the entire object
            services.AddSingleton(documentationOptions);
        }
    }
}
