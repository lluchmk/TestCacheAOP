using Core.AOP.ProxyGeneration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddTransient<TService>(_p => ProxyResolver.GenerateProxy<TImplementation>(_p));

        public static IServiceCollection AddScopedWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddScoped<TService>(_p => ProxyResolver.GenerateProxy<TImplementation>(_p));

        public static IServiceCollection AddSingletonWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddSingleton<TService>(_p => ProxyResolver.GenerateProxy<TImplementation>(_p));
    }
}
