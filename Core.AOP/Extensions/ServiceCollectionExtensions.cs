﻿using System;
using System.Collections.Generic;
using System.Text;

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
        {
            services.AddTransient<TService>(_p =>
            {
                var generator = new ProxyGenerator();
                var interceptorsDef = _p.GetService<InterceptorAssociationCollection>();

                List<IInterceptor> interceptors = new List<IInterceptor>();
                foreach (var entry in interceptorsDef)
                {
                    if (typeof(TImplementation).GetCustomAttributes(entry.attributeType, false).Length > 0)
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