using Cache.Core.AOP.Attributes;
using Cache.Core.Interfaces;
using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace Cache.Core.AOP.Interceptors
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

            if (_cache.Exists(cacheKey))
            {
                var cachedValue = _cache.Get(
                    cacheKey,
                    invocation.MethodInvocationTarget.ReturnType,
                    cacheAttribute.IsSlidingExpiration ? (TimeSpan?)cacheAttribute.TTL : null);

                invocation.ReturnValue = cachedValue;
                return;
            }

            invocation.Proceed();
            var value = invocation.ReturnValue;
            _cache.Set(cacheKey, value, cacheAttribute.TTL);
        }
    }
}
