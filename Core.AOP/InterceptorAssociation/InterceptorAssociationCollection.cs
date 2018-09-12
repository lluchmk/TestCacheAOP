using System.Collections.Generic;

namespace Core.AOP
{
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
