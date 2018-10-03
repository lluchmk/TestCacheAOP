using Cache.Core.AOP.Interceptors;
using Core.AOP.Atributes;
using System;

namespace Cache.Core.AOP.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheableAttribute : InterceptableAttribute
    {
        public override Type InterceptorType => typeof(CacheInterceptor);
    }
}
