using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class AttributeService
    {
        private NHibernate.ISession session;

        public AttributeService(NHibernate.ISession session)
        {
            // TODO: Complete member initialization
            this.session = session;
        }
        internal void StoreAttributeValue(System.Collections.IDictionary attributeValues)
        {
        }

        internal void StoreRole(string p, string ActorId)
        {
        }
    }
}
