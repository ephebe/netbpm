using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetBpm.Util.Client;
using NetBpm.Util.EComp;
using NHibernate;
using NetBpm.Util.DB;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Execution
{
    public class ProcessExecutionApplicationService : IProcessExecutionApplicationService
    {
        private MyProcessDefinitionService processDefinitionService = null;
        public IProcessInstance StartProcessInstance(long processDefinitionId)
        {
            return StartProcessInstance(processDefinitionId, null, null, null);
        }

        public IProcessInstance StartProcessInstance(long processDefinitionId, IDictionary attributeValues)
        {
            return StartProcessInstance(processDefinitionId, attributeValues, null, null);
        }

        public IProcessInstance StartProcessInstance(long processDefinitionId, IDictionary attributeValues, string transitionName)
        {
            return StartProcessInstance(processDefinitionId, attributeValues, transitionName, null);
        }

        public IProcessInstance StartProcessInstance(long processDefinitionId, IDictionary attributeValues, string transitionName, Relations relations)
        {
            IProcessInstance processInstance = null;
            IOrganisationService organisationService = null;
           
            using (ISession session = NHibernateHelper.OpenSession())
            {
                DbSession nhSession = new DbSession(session);
                organisationService = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));
                //ProcessDefinitionImpl processDefinition = processDefinitionService.GetProcessDefinition(processDefinitionId);
                //ProcessInstanceImpl processInstance = new ProcessInstanceImpl();
                //delegateService.RunActionsForEvent(EventType.BEFORE_PERFORM_OF_ACTIVITY, startState.Id, executionContext,dbSession);
                //logRepository.Add(new Log(){})
                //processInstance.StartState().CheckAccess(attributeValues)
                //delegateService.RunActionsForEvent(EventType.BEFORE_PERFORM_OF_ACTIVITY, startState.Id, executionContext,dbSession);
                //TransitionService.GetTransiation(name,processInstance.StartState())
                //TransitionService.processTransition()
            }

            return processInstance;
        }
    }
}
