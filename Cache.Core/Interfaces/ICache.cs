using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Core.Interfaces
{
    // TODO: Non-generic mehtodswith Type parameter
    public interface ICache
    {
        dynamic GetUnderlyingDatabase();

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
