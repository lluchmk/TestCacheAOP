using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using Cache.Core.Interfaces;

namespace TestCache.AOP
{
    public interface ICacheable
    { }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class CacheAttribute : Attribute
    {
        public readonly string Key;
        public readonly TimeSpan TTL;
        
        public CacheAttribute(string Key, string TTL)
        {
            this.Key = Key;
            this.TTL = TimeSpan.Parse(TTL);
        }
    }

    public class CacheInterceptor : IInterceptor
    {
        private ICache _cache;

        public CacheInterceptor(ICache cache)
        {
            _cache = cache;
        }

        // TODO: Use generic methos for cache
        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

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

    public class CacheProxyGeneratorHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
            Console.WriteLine($"Method {memberInfo.Name} could not be cached since it's not declared as virtual or because it does not return a value");
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return methodInfo.ReturnType != typeof(void) &&
                methodInfo.GetCustomAttribute(typeof(CacheAttribute)) != null;
        }
    }
}
