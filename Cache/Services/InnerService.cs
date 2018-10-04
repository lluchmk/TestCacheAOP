using Cache.Core.AOP.Attributes;

namespace Cache.Services
{
    public interface IInnerService
    {
        string Do(SimpleClass obj, int id);
    }

    [Cacheable]
    public class InnerService : IInnerService
    {

        [Cache("value_outId:{obj.Id}_inId:{obj.Inner.Id}_id:{id}", "00:00:30")]
        public virtual string Do(SimpleClass obj, int id)
        {
            return $"Inner service for outId={obj.Id}, inId={obj.Inner.Id}, id=id";
        }
    }

    public class SimpleClass
    {
        public int Id { get; set; }
        public SimpleInnerClass Inner { get; set; }
    }

    public class SimpleInnerClass
    {
        public int Id { get; set; }
    }
}
