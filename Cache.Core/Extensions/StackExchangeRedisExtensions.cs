using System;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace StackExchange.Redis
{
    public static class StackExchangeRedisExtensions
    {
        public static T GetDeserialized<T>(this IDatabase _database, string key)
        {
            var cachedValue = _database.StringGet(key);

            return !cachedValue.IsNull ?
                DeserializeObject<T>(cachedValue) :
                default(T);
        }

        public static async Task<T> GetDeserializedAsync<T>(this IDatabase _database, string key)
        {
            var cachedValue = await _database.StringGetAsync(key);

            return !cachedValue.IsNull ?
                DeserializeObject<T>(cachedValue) :
                default(T);
        }

        public static object GetDeserialized(this IDatabase _database, string key, Type type)
        {
            var cachedValue = _database.StringGet(key);

            return !cachedValue.IsNull ?
                DeserializeObject(cachedValue, type) :
                GetDefaultValue(type);
        }

        public static async Task<object> GetDeserializedAsync(this IDatabase _database, string key, Type type)
        {
            var cachedValue = await _database.StringGetAsync(key);

            return !cachedValue.IsNull ?
                DeserializeObject(cachedValue, type) :
                GetDefaultValue(type);
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ?
                Activator.CreateInstance(type) :
                null;
        }
    }
}
