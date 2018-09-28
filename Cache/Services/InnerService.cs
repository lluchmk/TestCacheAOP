using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cache.Core.AOP.Attributes;
using Cache.Core.AOP.Interceptors.Cache;

namespace Cache.Services
{
    public interface IInnerService
    {
        string Do(int id);
    }

    [Cacheable]
    public class InnerService : IInnerService
    {

        [Cache("value_id:{id}", "00:00:30")]
        public virtual string Do(int id)
        {
            return $"Inner service for {id}";
        }
    }
}
