using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StackExchange.Redis;

namespace TestCache.AOP
{
    public interface ICache
    {
        // TODO: Generic methods with serialization
        // TODO: Explore better ways to cleanly expose IDatabase

        IDatabase GetUnderlyingDatabase();

        bool KeyExists(string key);

        void AddString(string key, string value, TimeSpan ttl);

        string GetString(string key);
    }

    public class Cache : ICache
    {
        private IDatabase _cache;
        
        public Cache(IDatabase cache)
        {
            _cache = cache;
        }

        public IDatabase GetUnderlyingDatabase()
        {
            return _cache;
        }

        public bool KeyExists(string key)
        {
            return _cache.KeyExists(key);
        }

        public void AddString(string key, string value, TimeSpan ttl)
        {
            _cache.StringSet(key, value, ttl);
        }

        public string GetString(string key)
        {
            return _cache.StringGet(key);
        }
    }
}
