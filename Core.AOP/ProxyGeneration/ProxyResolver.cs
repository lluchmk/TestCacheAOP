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
        internal static T ResolveProxy<T>(IServiceProvider serviceProvider)
            where T : class
        {
            var interceptors = ResolveInterceptors<T>(serviceProvider);
            return GenerateProxy<T>(serviceProvider, interceptors);
        }

        private static IInterceptor[] ResolveInterceptors<T>(IServiceProvider serviceProvider)
        {
            var interceptorTypes = typeof(T).GetCustomAttributes<InterceptableAttribute>().Select(attr => attr.InterceptorType);
            return GenerateInterceptors(serviceProvider, interceptorTypes);
        }

        private static IInterceptor[] GenerateInterceptors(IServiceProvider serviceProvider, IEnumerable<Type> interceptorTypes)
        {
            var interceptors = new IInterceptor[interceptorTypes.Count()];
            for (int i = 0; i < interceptors.Length; i++)
            {
                interceptors[i] = GenerateInterceptor(serviceProvider, interceptorTypes.ElementAt(i));
            }
            return interceptors;
        }

        private static IInterceptor GenerateInterceptor(IServiceProvider serviceProvider, Type interceptorType)
        {
            var interceptorCtor = interceptorType.GetConstructors()[0];
            var interceptorParams = ResolveConstructorParameters(serviceProvider, interceptorCtor);
            return (IInterceptor)interceptorCtor.Invoke(interceptorParams);
        }

        private static T GenerateProxy<T>(IServiceProvider serviceProvider, IInterceptor[] interceptors)
        {
            var proxyGenerator = new ProxyGenerator();
            var ctrParams = ResolveTypeConstructorParameters(serviceProvider, typeof(T));
            return (T)proxyGenerator.CreateClassProxy(typeof(T), ctrParams, interceptors.ToArray());
        }

        private static object[] ResolveConstructorParameters(IServiceProvider provider, ConstructorInfo ctor)
        {
            var ctrParams = ctor.GetParameters();
            var resolvedParams = new object[ctrParams.Count()];
            for (int i = 0; i < resolvedParams.Length; i++)
            {
                resolvedParams[i] = ActivatorUtilities.GetServiceOrCreateInstance(provider, ctrParams[i].ParameterType);
            }
            return resolvedParams;
        }

        private static object[] ResolveTypeConstructorParameters(IServiceProvider serviceProvider, Type type)
        {
            var ctor = type.GetConstructors()[0];
            return ResolveConstructorParameters(serviceProvider, ctor);
        }
    }
}
