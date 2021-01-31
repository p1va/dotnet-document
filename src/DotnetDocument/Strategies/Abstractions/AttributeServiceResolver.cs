using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies.Abstractions
{
    /// <summary>
    /// The attribute service resolver class
    /// </summary>
    /// <seealso cref="IServiceResolver{TService}" />
    public class AttributeServiceResolver<TService> : IServiceResolver<TService>
    {
        /// <summary>
        /// The provider
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeServiceResolver" /> class
        /// </summary>
        /// <param name="provider">The provider</param>
        public AttributeServiceResolver(IServiceProvider provider) =>
            _provider = provider;

        /// <summary>
        /// Resolves the key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The service</returns>
        public TService? Resolve(string key)
        {
            var logger = _provider
                .GetService<ILoggerFactory>()
                .CreateLogger<AttributeServiceResolver<TService>>();

            logger.LogTrace("Resolving service {Type} with key {Key}", typeof(TService), key);

            var service = _provider
                .GetServices<TService>()
                .FirstOrDefault(s => s!
                    .GetType()
                    .GetCustomAttributes(false)
                    .OfType<StrategyAttribute>()
                    .Any(a => a.Key == key));

            if (service is null)
                logger.LogWarning("No {ServiceType} implementation resolved matching {KeyType} key: '{Key}'",
                    typeof(TService).Name, key.GetType().Name, key);
            else
                logger.LogTrace("Resolved implementation of {ServiceType} with key '{Key}': {ImplementationType}",
                    typeof(TService).Name, key, service.GetType().Name);

            return service;
        }
    }
}
