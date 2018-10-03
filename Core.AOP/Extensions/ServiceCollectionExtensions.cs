using Castle.DynamicProxy;
using Core.AOP.Atributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientAOP<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddTransient<TService>(_p => GenerateProxy<TImplementation>(_p));

        public static IServiceCollection AddScopedAOP<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddScoped<TService>(_p => GenerateProxy<TImplementation>(_p));


        public static IServiceCollection AddSingletonAOP<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
            => services.AddSingleton<TService>(_p => GenerateProxy<TImplementation>(_p));

        private static T GenerateProxy<T>(IServiceProvider serviceProvider)
            where T : class
        {
            var generator = new ProxyGenerator();

            List<IInterceptor> interceptors = new List<IInterceptor>();
            var interceptorTypes = typeof(T).GetCustomAttributes<InterceptableAttribute>().Select(attr => attr.InterceptorType);
            foreach (var interceptorType in interceptorTypes)
            {
                var ctr = interceptorType.GetConstructors()[0];
                var interceptorParams = ResolveConstructorParameters(serviceProvider, interceptorType, ctr);
                var interceptor = (IInterceptor)ctr.Invoke(interceptorParams);
                interceptors.Add(interceptor);
            }

            var ctrParams = ResolveConstructorParameters(serviceProvider, typeof(T));
            var proxy = (T)generator.CreateClassProxy(typeof(T), ctrParams, interceptors.ToArray());
            return proxy;
        }

        private static object[] ResolveConstructorParameters(IServiceProvider provider, Type t, ConstructorInfo ctr = null)
        {
            ctr = ctr ?? t.GetConstructors().Single();
            var ctrParams = ctr.GetParameters();
            var resolvedParams = new object[ctrParams.Count()];
            for (int i = 0; i < resolvedParams.Length; i++)
            {
                resolvedParams[i] = ActivatorUtilities.GetServiceOrCreateInstance(provider, ctrParams[i].ParameterType);
            }
            return resolvedParams;
        }
    }
}
