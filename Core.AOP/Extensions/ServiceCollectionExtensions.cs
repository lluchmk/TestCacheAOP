using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Castle.DynamicProxy;

using Core.AOP;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInterceptor<TAttribute, TInterceptor>(this IServiceCollection services)
            where TAttribute : class
            where TInterceptor : class, IInterceptor
        {
            var interceptorsDictionary = services.BuildServiceProvider().GetService<InterceptorAssociationCollection>();
            if (interceptorsDictionary == null)
            {
                interceptorsDictionary = new InterceptorAssociationCollection();
                services.AddSingleton(interceptorsDictionary);
            }
            interceptorsDictionary.AddEntry<TAttribute, TInterceptor>();
            return services;
        }

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
            var interceptorsDef = serviceProvider.GetService<InterceptorAssociationCollection>();

            List<IInterceptor> interceptors = new List<IInterceptor>();
            foreach (var entry in interceptorsDef)
            {
                if (typeof(T).GetCustomAttributes(entry.attributeType, false).Any())
                {
                    var ctr = entry.interceptorType.GetConstructors()[0];
                    var interceptorParams = CtrParams(serviceProvider, entry.interceptorType);
                    var interceptor = (IInterceptor)ctr.Invoke(interceptorParams);
                    interceptors.Add(interceptor);
                }
            }

            var ctrParams = CtrParams(serviceProvider, typeof(T));
            var proxy = (T)generator.CreateClassProxy(typeof(T), ctrParams, interceptors.ToArray());
            return proxy;
        }

        private static object[] CtrParams(IServiceProvider provider, Type t)
        {
            var ctr = t.GetConstructors().Single();
            var ctrParams = ctr.GetParameters();
            var ret = new object[ctrParams.Count()];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ActivatorUtilities.GetServiceOrCreateInstance(provider, ctrParams[i].ParameterType);
            }
            return ret;
        }
    }
}
