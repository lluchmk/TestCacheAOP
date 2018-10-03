using Cache.Core.AOP.Attributes;
using System.Collections.Generic;

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
        private readonly IEnumerable<string> _values = new string[] { "value1", "value2" };

        private readonly IInnerService _innerService;

        public ValuesService(IInnerService innerService)
        {
            _innerService = innerService;
        }

        public IEnumerable<string> GetValues()
        {
            _innerService.Do(1);
            return _values;
        }

        [Cache("value_id:{id}", "00:00:30")]
        public virtual string Get(int id)
        {
            return $"value{id}";
        }
    }
}
