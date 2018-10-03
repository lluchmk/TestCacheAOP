using System;
using System.Threading.Tasks;

namespace Cache.Core.Interfaces
{
    public interface ICache
    {
        dynamic GetUnderlyingDatabase();

        bool Exists(string key);

        Task<bool> ExistsAsync(string key);

        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        object Get(string key, Type type);

        Task<object> GetAsync(string key, Type type);

        void Set<T>(string key, T value, TimeSpan ttl);

        Task SetAsync<T>(string key, T value, TimeSpan ttl);

        void Remove(string key);

        Task RemoveAsync(string key);
    }
}
