﻿using System;

namespace Cache.Core.AOP.Attributes
{
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
}
