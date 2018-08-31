using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

using Castle.DynamicProxy;

using TestCache.AOP;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CacheStartupExtensions
    {
        private static IServiceCollection AddRedis(this IServiceCollection services, string connectionString) =>
            services.AddSingleton(_provider => ConnectionMultiplexer.Connect(connectionString).GetDatabase());
        /*public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config, string configurationSection = "String") =>
            services.AddSingleton(_provider => {
                
                // TODO: get configuration from config
                var redisConfSection = config.GetSection(configurationSection);
                var connectionOptions = new ConfigurationOptions();
                foreach (var endPoint in redisConfSection.GetSection("EndPoints").Get<string[]>())
                    connectionOptions.EndPoints.Add(endPoint);
                connectionOptions.AbortOnConnectFail = redisConfSection["AbbortOnFail"] ?? connectionOptions.AbortOnConnectFail;

                return ConnectionMultiplexer.Connect(connectionOptions).GetDatabase();
            });*/

        public static IServiceCollection AddCache(this IServiceCollection services, string connectionString) =>
            services.AddSingleton<ICache>(_p => new TestCache.AOP.Cache(ConnectionMultiplexer.Connect(connectionString).GetDatabase()));
    }

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
