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
                .CreateLogger();

            // Declare a new service collection
            var services = new ServiceCollection();

            // Configure service
            ConfigureServices(services);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the logger
            var logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("dotnet-document");

            // Parse command line args
            return Parser.Default
                .ParseArguments<ApplyCommandArgs, ConfigCommandArgs>(args)
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

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("dotnet-document.yml", optional: true)
                .AddYamlFile("dotnet-document.yaml", optional: true)
                .Build();

            // Add app configuration
            services.Configure<DotnetDocumentOptions>(configuration.GetSection("documentation"));

            // Add the commands
            services.AddTransient<ICommand<ApplyCommandArgs>, ApplyCommand>();
            services.AddTransient<ICommand<ConfigCommandArgs>, ConfigCommand>();
        }
    }
}
