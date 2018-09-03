using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cache.Core.AOP.Attributes;
using Cache.Core.AOP.Interceptors.Cache;

namespace Cache.Services
{
    public interface IValuesService
    {
        IEnumerable<string> GetValues();

        string Get(int id);
    }

    [Cacheable]
    public class ValuesService : IValuesService
    {
        private IEnumerable<string> _values = new string[] { "value1", "value2" };

        public IEnumerable<string> GetValues()
        {
            return _values;
        }

        [Cache("value_id:{id}", "00:00:30")]
        public virtual string Get(int id)
        {
            return $"value{id}";
        }
    }
}
