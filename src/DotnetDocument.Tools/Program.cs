using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Commands;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetDocument.Tools
{
    public class Program
    {
        public static IDocumentationStrategy Resolve(SyntaxKind kind, IServiceProvider provider)
        {
            var logger = provider
                .GetService<ILoggerFactory>()
                .CreateLogger<IDocumentationStrategy.ServiceResolver>();

            logger.LogTrace("Resolving documentation strategy for {Kind}", kind);

            var documentationStrategy = provider
                .GetServices<IDocumentationStrategy>()
                .FirstOrDefault(o => o.GetKind() == kind);

            if (documentationStrategy is null)
            {
                logger.LogWarning("No documentation strategy resolved for {Kind}", kind);
            }
            else
            {
                logger.LogTrace("Resolved {DocumentationStrategy} for {Kind}", documentationStrategy?.GetType(), kind);
            }

            return documentationStrategy;
        }

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
                .AddTransient<IDocumentationStrategy, PropertyDocumentationStrategy>();

            // Add the service resolver
            services
                .AddTransient<IDocumentationStrategy.ServiceResolver>(provider => kind => Resolve(kind, provider));

            // Add syntax walker
            services.AddTransient(provider =>
            {
                // Retrieve the list of supported SyntaxKinds from the DI
                var supportedDocumentationKinds = provider
                    .GetServices<IDocumentationStrategy>()
                    .Select(s => s.GetKind());

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
            services.AddTransient<DocumentCommand>();
            services.AddTransient<ConfigCommand>();
        }

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "{Message:lj}{NewLine}",
                    theme: AnsiConsoleTheme.None)
                .CreateLogger();

            var services = new ServiceCollection();

            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("dotnet-document");

            return Parser.Default
                .ParseArguments<DocumentCommandArgs, ConfigCommandArgs>(args)
                .MapResult(
                    (DocumentCommandArgs opts) => HandleDocumentCommand(opts, serviceProvider),
                    (ConfigCommandArgs opts) => HandleConfigCommand(opts, serviceProvider),
                    errors => (int)ExitCode.ArgsParsingError);
        }

        private static int HandleDocumentCommand(DocumentCommandArgs opts, IServiceProvider serviceProvider) =>
            (int)serviceProvider.GetService<DocumentCommand>().Run(opts);

        private static int HandleConfigCommand(ConfigCommandArgs opts, IServiceProvider serviceProvider) =>
            (int)serviceProvider.GetService<ConfigCommand>().Run(opts);
    }
}
