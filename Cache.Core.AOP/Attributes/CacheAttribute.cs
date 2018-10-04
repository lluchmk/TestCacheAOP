using System;

namespace Cache.Core.AOP.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class CacheAttribute : Attribute
    {
        public readonly string Key;
        public readonly TimeSpan TTL;
        public readonly bool IsSlidingExpiration;

        public CacheAttribute(string Key, string TTL, bool IsSlidingExpiration = false)
        {
            this.Key = Key;
            this.TTL = TimeSpan.Parse(TTL);
            this.IsSlidingExpiration = IsSlidingExpiration;
        }
    }
}
