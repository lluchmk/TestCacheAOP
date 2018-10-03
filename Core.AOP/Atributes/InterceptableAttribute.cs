using System;

namespace Core.AOP.Atributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class InterceptableAttribute : Attribute
    {
        public abstract Type InterceptorType { get; }
    }
}
