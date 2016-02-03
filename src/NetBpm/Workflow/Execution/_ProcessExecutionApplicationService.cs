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
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Delegation;
using System.Threading;

namespace NetBpm.Workflow.Execution
{
    public class ProcessExecutionApplicationService : IProcessExecutionApplicationService
    {
        private MyProcessDefinitionService myProcessDefinitionService = null;
        private DelegationService delegateService = null;
        private TransitionService transitionService = null;
        private AttributeService attributeService = null;
        private TransitionRepository transitionRepository = null;
        private ProcessInstanceRepository processInstanceRepository = null;

        public ProcessExecutionApplicationService() 
        {
            myProcessDefinitionService = new MyProcessDefinitionService();
            delegateService = new DelegationService();
          
          
            transitionRepository = TransitionRepository.Instance;
            processInstanceRepository = ProcessInstanceRepository.Instance;
        }

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
            ProcessInstanceImpl processInstance = null;
            IOrganisationService organisationService = null;
           
            using (ISession session = NHibernateHelper.OpenSession())
            {
                DbSession nhSession = new DbSession(session);
                organisationService = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));
                ProcessDefinitionImpl processDefinition = myProcessDefinitionService.GetProcessDefinition(processDefinitionId, nhSession);
                processInstance = new ProcessInstanceImpl(ActorId,processDefinition);
                processInstanceRepository.Save(processInstance,nhSession);//到這裏應該存了ProcessInstance,RootFlow
                processDefinition.StartState.CheckAccess(attributeValues);

                attributeService = new AttributeService(session);
                attributeService.StoreAttributeValue(attributeValues);//儲存傳入的欄位值
                attributeService.StoreRole(((ActivityStateImpl)processDefinition.StartState).ActorRoleName,ActorId);

                //flow的node推進到下一關卡
                //flow的actor=解析出來的actor.Id
                transitionService = new TransitionService(session);
                TransitionImpl transitionTo = transitionService.GetTransition(transitionName, processDefinition.StartState,nhSession);
                transitionService.ProcessTransition(transitionTo, (FlowImpl)processInstance.RootFlow, nhSession);

                session.Flush();
            }

            return processInstance;
        }

        public String ActorId
        {
            get { return Thread.CurrentPrincipal.Identity.Name; }
        }
    }
}
