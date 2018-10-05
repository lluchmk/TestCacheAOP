using Castle.DynamicProxy;
using Core.AOP.Atributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.AOP.ProxyGeneration
{
    internal static class ProxyResolver
    {
        internal static T GenerateProxy<T>(IServiceProvider serviceProvider)
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

        internal static object[] ResolveConstructorParameters(IServiceProvider provider, Type t, ConstructorInfo ctr = null)
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
