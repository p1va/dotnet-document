using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotnetDocument.Strategies.Abstractions
{
    public class AttributeServiceResolver<TService> : IServiceResolver<TService>
    {
        private readonly IServiceProvider _provider;

        public AttributeServiceResolver(IServiceProvider provider) =>
            (_provider) = (provider);

        public TService Resolve(string key)
        {
            var logger = _provider
                .GetService<ILoggerFactory>()
                .CreateLogger<AttributeServiceResolver<TService>>();

            logger.LogTrace("Resolving service {Type} with key {Key}", typeof(TService), key.ToString());

            var service = _provider
                .GetServices<TService>()
                .FirstOrDefault(s => s
                    .GetType()
                    .GetCustomAttributes(inherit: false)
                    .OfType<StrategyAttribute>()
                    .Any(a => a.Key == key));

            if (service is null)
            {
                logger.LogWarning("No {ServiceType} implementation resolved matching {KeyType} key: '{Key}'",
                    typeof(TService).Name, key.GetType().Name, key);
            }
            else
            {
                logger.LogTrace(
                    "Resolved implementation of {ServiceType} with key '{Key}': {ImplementationType}",
                    typeof(TService).Name, key, service.GetType().Name);
            }

            return service;
        }
    }
}
