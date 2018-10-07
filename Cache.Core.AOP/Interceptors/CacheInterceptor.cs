using Cache.Core.AOP.Attributes;
using Cache.Core.AOP.Exceptions;
using Cache.Core.Interfaces;
using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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

            var methodParameters = invocation.MethodInvocationTarget.GetParameters();
            var cacheKey = ReplaceCacheKeyParameters(cacheAttribute.Key, methodParameters, invocation.Arguments);
            if (_cache.Exists(cacheKey))
            {
                var cachedValue = _cache.Get(cacheKey, invocation.MethodInvocationTarget.ReturnType);

                if (cacheAttribute.IsSlidingExpiration)
                {
                    _cache.SetExpirationTime(cacheKey, cacheAttribute.TTL);
                }

                invocation.ReturnValue = cachedValue;
                return;
            }

            invocation.Proceed();
            var value = invocation.ReturnValue;
            _cache.Set(cacheKey, value, cacheAttribute.TTL);
        }

        private string ReplaceCacheKeyParameters(string cacheKey, ParameterInfo[] invocationParameters, object[] invocationArguments)
        {
            string replacedCacheKey = cacheKey;

            var cacheKeyToReplaceRegex = new Regex("{([^|]*?)}");
            var matches = cacheKeyToReplaceRegex.Matches(cacheKey);

            foreach (Match match in matches)
            {
                var stringToReplace = match.Groups[0].Value;
                var propertyName = match.Groups[1].Value;

                if (propertyName.Contains('.'))
                {
                    var navigationParts = new Span<string>(propertyName.Split('.'));
                    var firstPart = navigationParts[0];

                    var parameterIndex = invocationParameters.SingleOrDefault(p => p.Name == firstPart)?.Position ??
                        throw new InvalidCacheKeyException($"Invalid parameter name: {firstPart}");

                    var parameterValue = GetInnerPropertyValue(navigationParts.Slice(1), invocationArguments[parameterIndex]);
                    replacedCacheKey = replacedCacheKey.Replace(stringToReplace, parameterValue);
                }
                else
                {
                    var parameterIndex = invocationParameters.SingleOrDefault(p => p.Name == propertyName)?.Position ??
                        throw new InvalidCacheKeyException($"Invalid parameter name: {propertyName}");

                    var parameterValue = invocationArguments[parameterIndex];
                    replacedCacheKey = replacedCacheKey.Replace(stringToReplace, parameterValue.ToString());
                }
            }

            return replacedCacheKey;
        }

        private string GetInnerPropertyValue(Span<string> navigationParts, object obj)
        {
            var propertyValue = obj.GetType().GetProperty(navigationParts[0])?.GetValue(obj, null) ??
                throw new InvalidCacheKeyException($"Invalid navigation property: {navigationParts[0]}");

            return navigationParts.Length > 1 ?
                GetInnerPropertyValue(navigationParts.Slice(1), propertyValue) :
                propertyValue.ToString();
        }
    }
}
