using NetBpm.Util.DB;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class AttributeService
    {
        private DbSession session;
        private AttributeRepository attributeRepository = AttributeRepository.Instance;
        private FlowImpl currentScope = null;

        public AttributeService(FlowImpl scope,DbSession session)
        {
            // TODO: Complete member initialization
            this.session = session;
            currentScope = scope;
        }

        public void StoreAttributeValue(System.Collections.IDictionary attributeValues)
        {
            if (attributeValues != null)
            {
                // loop over all provided attributeValues
                IEnumerator iter = attributeValues.GetEnumerator();
                while (iter.MoveNext())
                {
                    DictionaryEntry entry = (DictionaryEntry)iter.Current;
                    String attributeName = (String)entry.Key;
                    // and store it
                    SetAttribute(attributeName, entry.Value);
                }
            }
        }

        public void StoreRole(IActor actor, ActivityStateImpl activityState)
        {
            String role = activityState.ActorRoleName;
            if ((Object)role != null)
            {
                //log.Debug("assigning " + authenticatedActor + " to role " + role);
                SetAttribute(role, actor);
            }
        }

        private void SetAttribute(String name, Object valueObject)
        {
            AttributeInstanceImpl attributeInstance = attributeRepository.FindAttributeInstanceInScope(name, currentScope,this.session);
            if (attributeInstance != null)
            {
                attributeInstance.SetValue(valueObject);
                //this.AddLogDetail(new AttributeUpdateImpl(attributeInstance));
            }
        }
    }
}
