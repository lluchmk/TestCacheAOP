using System;
using System.Collections.Generic;
using System.Text;

using Castle.DynamicProxy;

using Cache.Core.Interfaces;
using Cache.Core.AOP.Interceptors.Cache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientCached<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>(provider => {
                var generator = new ProxyGenerator();
                var proxy = generator.CreateClassProxy<TImplementation>(new CacheInterceptor(provider.GetService<ICache>()));
                return proxy;
            });

            return services;
        }
    }
}
