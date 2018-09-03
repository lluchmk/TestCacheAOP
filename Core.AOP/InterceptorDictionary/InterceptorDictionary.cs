using System;
using System.Collections.Generic;
using System.Text;

namespace Core.AOP
{
    // TODO: Separate into different files
    public class InterceptorAssociation
    {
        public Type interfaceType;
        public Type interceptorType;
    }

    public class InterceptorAssociationCollection : List<InterceptorAssociation>
    {
        public void AddEntry<TInterface, TInterceptor>()
        {
            Add(new InterceptorAssociation
            {
                interfaceType = typeof(TInterface),
                interceptorType = typeof(TInterceptor)
            });
        }
    }
}
