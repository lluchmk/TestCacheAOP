using System;
using System.Collections.Generic;
using System.Text;

using Castle.DynamicProxy;

using Core.AOP;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInterceptor<TInterface, TInterceptor>(this IServiceCollection services)
            where TInterface : class
            where TInterceptor : class, IInterceptor
        {
            var interceptorsDictionary = services.BuildServiceProvider().GetService<InterceptorAssociationCollection>();
            if (interceptorsDictionary == null)
            {
                interceptorsDictionary = new InterceptorAssociationCollection();
                services.AddSingleton(interceptorsDictionary);
            }
            interceptorsDictionary.AddEntry<TInterface, TInterceptor>();
            return services;
        }

        public static IServiceCollection AddTransientAOP<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
        {
            services.AddTransient<TService>(_p =>
            {
                var generator = new ProxyGenerator();
                var interceptorsDef = _p.GetService<InterceptorAssociationCollection>();

                List<IInterceptor> interceptors = new List<IInterceptor>();
                foreach (var entry in interceptorsDef)
                {
                    var interfaceName = entry.interfaceType.Name;
                    if (typeof(TImplementation).GetInterface(interfaceName) != null)
                    {
                        var ctr = entry.interceptorType.GetConstructors()[0];
                        var ctrParams = new List<object>();
                        foreach (var p in ctr.GetParameters())
                        {
                            ctrParams.Add(_p.GetService(p.ParameterType));
                        }
                        var interceptor = (IInterceptor)ctr.Invoke(ctrParams.ToArray());
                        interceptors.Add(interceptor);
                    }
                }

                var proxy = generator.CreateClassProxy<TImplementation>(interceptors.ToArray());
                return proxy;
            });

            return services;
        }
    }
}
