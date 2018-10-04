using System;

namespace Cache.Core.AOP.Exceptions
{
    public class InvalidCacheKeyException : Exception
    {
        public InvalidCacheKeyException()
            : base()
        { }

        public InvalidCacheKeyException(string message)
            : base(message)
        { }
    }
}
