using System;
using System.Collections.Generic;
using System.Text;

namespace Core.AOP
{
    // TODO: Separate into different files
    public class InterceptorAssociation
    {
        public Type attributeType;
        public Type interceptorType;
    }

    public class InterceptorAssociationCollection : List<InterceptorAssociation>
    {
        public void AddEntry<TAttribute, TInterceptor>()
        {
            Add(new InterceptorAssociation
            {
                attributeType = typeof(TAttribute),
                interceptorType = typeof(TInterceptor)
            });
        }
    }
}
