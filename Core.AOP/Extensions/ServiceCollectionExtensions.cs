using static Core.AOP.ProxyGeneration.ProxyResolver;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddTransient<TService>(_p => ResolveProxy<TImplementation>(_p));

        public static IServiceCollection AddScopedWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddScoped<TService>(_p => ResolveProxy<TImplementation>(_p));

        public static IServiceCollection AddSingletonWithInterceptors<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddSingleton<TService>(_p => ResolveProxy<TImplementation>(_p));
    }
}
