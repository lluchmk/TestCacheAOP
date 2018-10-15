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

            if (cacheAttribute is null)
            {
                invocation.Proceed();
                return;
            }

            var cacheKey = ReplaceCacheKeyParameters(cacheAttribute.Key, invocation);
            if (_cache.Exists(cacheKey))
            {
                var cachedValue =  GetFromCacheAndRefreshExpiration(cacheKey, invocation.MethodInvocationTarget.ReturnType, cacheAttribute);
                invocation.ReturnValue = cachedValue;
                return;
            }

            ProceedInvocationAndCache(ref invocation, cacheKey, cacheAttribute.TTL);
        }

        private string ReplaceCacheKeyParameters(string cacheKey, IInvocation invocation)
        {
            string replacedCacheKey = cacheKey;

            var invocationParameters = invocation.MethodInvocationTarget.GetParameters();

            var cacheKeyToReplaceRegex = new Regex("{([^|]*?)}");
            var matches = cacheKeyToReplaceRegex.Matches(cacheKey);

            foreach (Match match in matches)
            {
                replacedCacheKey = ReplaceMatch(invocationParameters, invocation.Arguments, replacedCacheKey, match);
            }

            return replacedCacheKey;
        }

        private string ReplaceMatch(ParameterInfo[] invocationParameters, object[] invocationArguments, string replacedCacheKey, Match match)
        {
            var stringToReplace = match.Groups[0].Value;
            var propertyName = match.Groups[1].Value;

            replacedCacheKey = IsNavigationProperty(propertyName) ?
                ReplaceNavigationProperty(invocationParameters, invocationArguments, replacedCacheKey, stringToReplace, propertyName) :
                ReplaceProperty(invocationParameters, invocationArguments, replacedCacheKey, stringToReplace, propertyName);

            return replacedCacheKey;
        }

        private static string ReplaceProperty(ParameterInfo[] invocationParameters, object[] invocationArguments, string replacedCacheKey, string stringToReplace, string propertyName)
        {
            var parameterIndex = invocationParameters.SingleOrDefault(p => p.Name == propertyName)?.Position ??
                                throw new InvalidCacheKeyException($"Invalid parameter name: {propertyName}");

            var parameterValue = invocationArguments[parameterIndex];
            replacedCacheKey = replacedCacheKey.Replace(stringToReplace, parameterValue.ToString());
            return replacedCacheKey;
        }

        private string ReplaceNavigationProperty(ParameterInfo[] invocationParameters, object[] invocationArguments, string replacedCacheKey, string stringToReplace, string propertyName)
        {
            var navigationParts = new Span<string>(propertyName.Split('.'));
            var firstPart = navigationParts[0];

            var parameterIndex = invocationParameters.SingleOrDefault(p => p.Name == firstPart)?.Position ??
                throw new InvalidCacheKeyException($"Invalid parameter name: {firstPart}");

            var parameterValue = GetInnerPropertyValue(navigationParts.Slice(1), invocationArguments[parameterIndex]);
            replacedCacheKey = replacedCacheKey.Replace(stringToReplace, parameterValue);
            return replacedCacheKey;
        }

        private bool IsNavigationProperty(string propertyName)
        {
            return propertyName.Contains('.');
        }

        private string GetInnerPropertyValue(Span<string> navigationParts, object obj)
        {
            var propertyValue = obj.GetType().GetProperty(navigationParts[0])?.GetValue(obj, null) ??
                throw new InvalidCacheKeyException($"Invalid navigation property: {navigationParts[0]}");

            return navigationParts.Length > 1 ?
                GetInnerPropertyValue(navigationParts.Slice(1), propertyValue) :
                propertyValue.ToString();
        }

        private object GetFromCacheAndRefreshExpiration(string cacheKey, Type type, CacheAttribute cacheAttribute)
        {
            var cachedValue = _cache.Get(cacheKey, type);

            if (cacheAttribute.IsSlidingExpiration)
            {
                _cache.SetExpirationTime(cacheKey, cacheAttribute.TTL);
            }

            return cachedValue;
        }

        private void ProceedInvocationAndCache(ref IInvocation invocation, string cacheKey, TimeSpan ttl)
        {
            invocation.Proceed();
            var invocationResponse = invocation.ReturnValue;
            _cache.Set(cacheKey, invocationResponse, ttl);
        }
    }
}
