using Cache.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace Cache.Core.Definitions
{
    public class RedisCache : ICache
    {
        private IDatabase _database;

        public RedisCache(IDatabase database)
        {
            _database = database;
        }

        public dynamic GetUnderlyingDatabase() => _database;

        public Type GetUnderlyingDatabaseType() => typeof(IDatabase);

        public bool Exists(string key)
        {
            return _database.KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public T Get<T>(string key)
        {
            return _database.GetDeserialized<T>(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _database.GetDeserializedAsync<T>(key);
        }

        public object Get(string key, Type type)
        {
            return _database.GetDeserialized(key, type);
        }

        public async Task<object> GetAsync(string key, Type type)
        {
            return await _database.GetDeserializedAsync(key, type);
        }

        public void Set<T>(string key, T value, TimeSpan ttl)
        {
            var serialized = SerializeObject(value);
            _database.StringSet(key, serialized, ttl);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
        {
            var serialized = SerializeObject(value);
            await _database.StringSetAsync(key, serialized, ttl);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public void SetExpirationTime(string key, TimeSpan ttl)
        {
            _database.KeyExpire(key, ttl);
        }

        public async Task SetExpirationTimeAsync(string key, TimeSpan ttl)
        {
            await _database.KeyExpireAsync(key, ttl);
        }
    }
}
