using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cache.Core.AOP.Attributes;

namespace Cache.Services
{
    public interface IValuesService
    {
        IEnumerable<string> GetValues();

        string Get(int id);
    }

    public class ValuesService : IValuesService
    {
        private IEnumerable<string> _values = new string[] { "value1", "value2" };

        public IEnumerable<string> GetValues()
        {
            return _values;
        }

        [Cache("value", "00:00:30")]
        public virtual string Get(int id)
        {
            return "value";
        }
    }
}
