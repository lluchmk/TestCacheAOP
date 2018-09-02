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

            var cachedValue = _cache.Get(cacheAttribute.Key, invocation.TargetType);

            if (cachedValue != null)
            {
                invocation.ReturnValue = cachedValue;
                return;
            }

            invocation.Proceed();
            var value = invocation.ReturnValue;
            _cache.Set(cacheAttribute.Key, value, cacheAttribute.TTL);
        }
    }
}
