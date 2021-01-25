using System.Diagnostics;
using System.IO;
using System.Linq;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Strategies.Abstractions;
using DotnetDocument.Syntax;
using DotnetDocument.Tools.Commands;
using DotnetDocument.Tools.Config;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotnetDocument.Tools
{
    public static class ServiceCollectionExtensions
    {
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
            services.AddTransient<ICommand<ApplyCommandArgs>, ApplyCommand>();
            services.AddTransient<ICommand<ConfigCommandArgs>, ConfigCommand>();
        }

        public static void ConfigureOptions(this IServiceCollection services, string? configFilePath = null)
        {
            var documentationOptions = new DocumentationOptions();

            if (!string.IsNullOrWhiteSpace(configFilePath))
            {
                Log.Logger.Debug("Loading config from file: '{ConfigFilePath}'", configFilePath);

                try
                {
                    documentationOptions = Yaml.Deserialize<DocumentationOptions>(configFilePath);
                }
                catch (FileNotFoundException e)
                {
                    Log.Logger.Error("No config file found at '{ConfigFilePath}'", configFilePath);
                    Log.Logger.Debug(e.Demystify().ToString());

                    // TODO: Exit code
                    throw;
                }
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
