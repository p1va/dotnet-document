using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Commands;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
            // services
            //     .AddLogging(log =>
            //     {
            //         log.SetMinimumLevel(LogLevel.Information);
            //         log.AddSimpleConsole(c =>
            //         {
            //             c.IncludeScopes = false;
            //             c.ColorBehavior = LoggerColorBehavior.Enabled;
            //             c.SingleLine = true;
            //         });
            //     });

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            // Add documentation strategies
            services
                .AddTransient<IFormatter, HumanizeFormatter>()
                .AddTransient<IDocumentationStrategy, ClassDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, InterfaceDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, EnumDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, EnumMemberDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, ConstructorDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, MethodDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy, PropertyDocumentationStrategy>()
                .AddTransient<IDocumentationStrategy.ServiceResolver>(provider => kind => Resolve(kind, provider));

            services.AddTransient(provider =>
            {
                var supportedDocumentationKinds = provider
                    .GetServices<IDocumentationStrategy>()
                    .Select(s => s.GetKind());

                return new DocumentationSyntaxWalker(supportedDocumentationKinds);
            });

            // Build configuration
            // var configuration = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     //.AddYamlFile("dotnet-document.yml", optional: true)
            //     //.AddYamlFile("dotnet-document.yaml", optional: true)
            //     .AddEnvironmentVariables()
            //     .Build();

            // Add app configuration
            services.Configure<DotnetDocumentOptions>(_ => new DotnetDocumentOptions());
            //services.Configure<DotnetDocumentOptions>(configuration.GetSection("documentation"));

            // Add the app
            services
                .AddTransient<DocumentCommand>();
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

            return Parser.Default.ParseArguments<CommandArgs>(args)
                .WithParsed(opts => HandleCommands(opts, logger, serviceProvider))
                .WithNotParsed(errors => HandleParseError(errors, logger))
                .MapResult(
                    result => (int)result.ExitCode,
                    errors => (int)ExitCode.ArgsParsingError);
        }

        private static void HandleCommands(CommandArgs opts, ILogger<Program> logger,
            IServiceProvider serviceProvider)
        {
            //handle options
            var exitCode = serviceProvider.GetService<DocumentCommand>().Run(opts);

            opts.ExitCode = exitCode;
        }

        private static void HandleParseError(IEnumerable<Error> errors, ILogger<Program> logger)
        {
            foreach (var error in errors)
            {
                logger.LogDebug("Error parsing the command: {Error}", error.Tag.ToString());
            }
        }
    }
}
