using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

using Castle.DynamicProxy;

using Cache.Core.Interfaces;
using TestCache.AOP;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CacheStartupExtensionsAOP
    {
        public static IServiceCollection AddTransientCached<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>(provider => {
                var generator = new ProxyGenerator();
                var options = new ProxyGenerationOptions(new CacheProxyGeneratorHook());
                var proxy = generator.CreateClassProxy<TImplementation>(options, new CacheInterceptor(provider.GetService<ICache>()));
                return proxy;
            });

            return services;
        }
    }
}
