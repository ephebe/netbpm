using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Execution.Impl;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class ProcessExecutionApplicationService
    {
        public IProcessInstance StartProcessInstance(Int64 processDefinitionId)
        {
            return StartProcessInstance(processDefinitionId, null, null, null);
        }

        public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues)
        {
            return StartProcessInstance(processDefinitionId, attributeValues, null, null);
        }

        public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName)
        {
            return StartProcessInstance(processDefinitionId, attributeValues, transitionName, null);
        }

        public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations)
        {
            ProcessDefinitionRepository processDefinitionRepository = ProcessDefinitionRepository.Instance;
            ProcessExecutionService processExecutionService = new ProcessExecutionService();
            
            using (ISession session = NHibernateHelper.OpenSession())
            {
                DbSession nhSession = new DbSession(session);
                ProcessDefinitionImpl processDefinition = (ProcessDefinitionImpl)processDefinitionRepository.GetProcessDefinition(processDefinitionId, null, nhSession);
                ProcessInstanceImpl processInstance = processExecutionService.Start(processDefinition, nhSession);
                TransitionImpl transition = null;//(TransitionImpl)processDefinition.StartState.ArrivingTransitions[transitionName];
                processExecutionService.ProcessTransition(transition, (FlowImpl)processInstance.RootFlow);
            }

            return null;
        }
    }
}
