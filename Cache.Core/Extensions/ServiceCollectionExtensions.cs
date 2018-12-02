using Cache.Core.Definitions;
using Cache.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtesions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString) =>
            services.AddSingleton<ICache>(_p => new RedisCache(ConnectionMultiplexer.Connect(connectionString).GetDatabase()));

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config, string configurationSectionName = "Redis") =>
            services.AddSingleton<ICache>(_provider =>
            {
                var configurationSection = config.GetSection(configurationSectionName);
                var redisConfigurationOptions = configurationSection.Get<ConfigurationOptions>();

                var endpointsSection = configurationSection.GetSection("EndPoints");
                foreach (var endpoint in endpointsSection.Get<string[]>())
                {
                    redisConfigurationOptions.EndPoints.Add(endpoint);
                }

                return new RedisCache(ConnectionMultiplexer.Connect(redisConfigurationOptions).GetDatabase());
            });
    }
}
