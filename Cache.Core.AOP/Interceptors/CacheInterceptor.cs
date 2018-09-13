using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Castle.DynamicProxy;

using Cache.Core.Interfaces;
using Cache.Core.AOP.Attributes;

namespace Cache.Core.AOP.Interceptors.Cache
{
    public class CacheInterceptor : IInterceptor
    {
        private ICache _cache;

        public CacheInterceptor(ICache cache)
        {
            _cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            if (cacheAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            var cacheKey = cacheAttribute.Key;
            var @params = invocation.MethodInvocationTarget.GetParameters();
            for (int i = 0; i < @params.Length; i++)
            {
                var paramName = @params[i].Name;
                var val = invocation.Arguments[i];
                cacheKey = cacheKey.Replace($"{{{paramName}}}", val.ToString());
            }

            var cachedValue = _cache.Get(cacheKey, invocation.MethodInvocationTarget.ReturnType);

            if (cachedValue != null)
            {
                invocation.ReturnValue = cachedValue;
                return;
            }

            invocation.Proceed();
            var value = invocation.ReturnValue;
            _cache.Set(cacheKey, value, cacheAttribute.TTL);
        }
    }
}
