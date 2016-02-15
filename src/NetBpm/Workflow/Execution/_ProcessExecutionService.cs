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
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Execution
{
    public class ProcessExecutionService
    {
        private MyProcessDefinitionService myProcessDefinitionService = null;
        private DelegationService delegateService = null;
        private TransitionService transitionService = null;
        private AttributeService attributeService = null;
        private TransitionRepository transitionRepository = null;
        private ProcessInstanceRepository processInstanceRepository = null;
        private TaskRepository taskRepository = null;
        private FlowRepository flowRepository = null; 

        public ProcessExecutionService() 
        {
            myProcessDefinitionService = new MyProcessDefinitionService();
            delegateService = new DelegationService();

            taskRepository = TaskRepository.Instance;
            transitionRepository = TransitionRepository.Instance;
            processInstanceRepository = ProcessInstanceRepository.Instance;
            flowRepository = FlowRepository.Instance;
        }

        public IProcessInstance StartProcessInstance(long processDefinitionId, IDictionary attributeValues = null, string transitionName = null, Relations relations = null)
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

                ExecutionContext executionContext = new ExecutionContext();
                //logRepository.CreateLog();
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

        public IList GetTaskList(String actorId,Relations relations = null)
        {
            IList taskLists = null;
            IOrganisationService organisationComponent = null;
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    DbSession dbSession = new DbSession(session);
                    organisationComponent = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));
                    IActor actor = organisationComponent.FindActorById(actorId);
                    taskLists = taskRepository.FindTasks(actorId, dbSession);
                }
            }
            finally
            {
                ServiceLocator.Instance.Release(organisationComponent);
            }
            return taskLists;
        }

        public IList PerformActivity(long flowId, IDictionary attributeValues = null, String transitionName=null, Relations relations=null)
        {
            if (string.IsNullOrEmpty(ActorId))
            {
                throw new AuthorizationException("you can't perform an activity because you are not authenticated");
            }

            IList flows = null;
            IOrganisationService organisationComponent = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    DbSession dbSession = new DbSession(session);
                    organisationComponent = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));
                    FlowImpl flow = flowRepository.GetFlow(flowId,dbSession);
                    ActivityStateImpl activityState = (ActivityStateImpl)flow.Node;

                    attributeService = new AttributeService(session);
                    attributeService.StoreAttributeValue(attributeValues);

                    transitionService = new TransitionService(session);
                    TransitionImpl transitionTo = transitionService.GetTransition(transitionName, activityState, dbSession);
                    transitionService.ProcessTransition(transitionTo, flow, dbSession);

                    ServiceLocator.Instance.Release(organisationComponent);
                    session.Flush();
                }
            }
            catch (ExecutionException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new SystemException("uncaught exception : " + e.Message, e);
            }
            finally
            {
                ServiceLocator.Instance.Release(organisationComponent);
            }
            return flows;
        }
    }
}
