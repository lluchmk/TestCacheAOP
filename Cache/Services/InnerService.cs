using Cache.Core.AOP.Attributes;

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
