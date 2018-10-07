using System;

namespace Cache.Core.AOP.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class CacheWithSlidingExpirationAttribute : CacheAttribute
    {
        public CacheWithSlidingExpirationAttribute(string Key, string TTL)
            : base (Key, TTL, true)
        {}
    }
}
