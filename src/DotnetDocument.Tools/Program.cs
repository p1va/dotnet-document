using System;
using System.Linq;
using System.Threading.Tasks;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Tools
{
    class Program
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
            services
                .AddLogging(c =>
                {
                    c.SetMinimumLevel(LogLevel.Trace);
                    c.AddConsole();
                });

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
                .AddTransient<App>();
        }

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("dotnet-format");

            await serviceProvider
                .GetService<App>()
                .Run(args);
        }
    }
}
