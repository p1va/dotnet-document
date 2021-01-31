using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Config;
using DotnetDocument.Tools.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotnetDocument.Tools
{
    /// <summary>
    /// The service collection extensions class
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the dotnet document using the specified services
        /// </summary>
        /// <param name="services">The services</param>
        public static void AddDotnetDocument(this IServiceCollection services)
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
                    AttributeServiceResolver<IDocumentationStrategy>>(provider =>
                    new AttributeServiceResolver<IDocumentationStrategy>(provider));

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
            services.AddTransient<IApplyDocumentHandler, ApplyDocumentHandler>();
            services.AddTransient<IDocumentConfigHandler, DocumentConfigHandler>();
        }

        /// <summary>
        /// Configures the from file using the specified services
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="configFilePath">The config file path</param>
        public static void ConfigureFromFile(this IServiceCollection services, string? configFilePath = null)
        {
            var documentationOptions = new DocumentationOptions();

            if (!string.IsNullOrWhiteSpace(configFilePath))
            {
                Log.Logger.Debug("Loading config from file: '{ConfigFilePath}'", configFilePath);

                documentationOptions = Yaml.Deserialize<DocumentationOptions>(configFilePath);
            }
            else
            {
                Log.Logger.Verbose("No config file provided. Using default settings");
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
