using Cache.Core.Definitions;
using Cache.Core.Interfaces;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtesions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString) =>
            services.AddSingleton<ICache>(_p => new RedisCache(ConnectionMultiplexer.Connect(connectionString).GetDatabase()));

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
    }
}
