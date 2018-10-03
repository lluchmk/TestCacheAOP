using System;
using System.Threading.Tasks;

namespace Cache.Core.Interfaces
{
    public interface ICache
    {
        dynamic GetUnderlyingDatabase();

        bool Exists(string key);

        Task<bool> ExistsAsync(string key);

        T Get<T>(string key, TimeSpan? slidingExpiration = null);

        Task<T> GetAsync<T>(string key, TimeSpan? slidingExpiration = null);

        object Get(string key, Type type, TimeSpan? slidingExpiration = null);

        Task<object> GetAsync(string key, Type type, TimeSpan? slidingExpiration = null);

        void Set<T>(string key, T value, TimeSpan ttl);

        Task SetAsync<T>(string key, T value, TimeSpan ttl);

        void Remove(string key);

        Task RemoveAsync(string key);
    }
}
