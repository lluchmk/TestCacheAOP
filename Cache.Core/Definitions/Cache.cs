using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cache.Core.Interfaces;
using StackExchange.Redis;
using static Newtonsoft.Json.JsonConvert;

namespace Cache.Core.Definitions
{
    public class Cache : ICache
    {
        private IDatabase _database;

        public Cache(IDatabase database)
        {
            _database = database;
        }

        public dynamic GetUnderlyingDatabase()
        {
            return _database;
        }
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
            var strValue = _database.StringGet(key);

            if (!strValue.IsNull)
            {
                var deserializedValue = DeserializeObject<T>(strValue);
                return deserializedValue;
            }

            return default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var strValue = await _database.StringGetAsync(key);

            if (!strValue.IsNull)
            {
                var deserializedValue = DeserializeObject<T>(strValue);
                return deserializedValue;
            }

            return default(T);
        }    
        
        public object Get(string key, Type type)
        {
            var strValue = _database.StringGet(key);

            if (!strValue.IsNull)
            {
                var deserializedValue = DeserializeObject(strValue, type);
                return deserializedValue;
            }

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        public async Task<object> GetAsync(string key, Type type)
        {
            var strValue = await _database.StringGetAsync(key);

            if (!strValue.IsNull)
            {
                var deserializedValue = DeserializeObject(strValue, type);
                return deserializedValue;
            }

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
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
    }
}
