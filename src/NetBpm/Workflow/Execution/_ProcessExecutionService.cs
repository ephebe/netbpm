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
        private IOrganisationService organisationService = null;
        private TransitionRepository transitionRepository = null;
        private ProcessInstanceRepository processInstanceRepository = null;
        private TaskRepository taskRepository = null;
        private FlowRepository flowRepository = null; 

        public ProcessExecutionService() 
        {
            myProcessDefinitionService = new MyProcessDefinitionService();
            delegateService = new DelegationService();
            organisationService = (IOrganisationService)ServiceLocator.Instance.GetService(typeof(IOrganisationService));

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
                using (var tran = session.BeginTransaction()) 
                {
                    DbSession dbSession = new DbSession(session);
                    ProcessDefinitionImpl processDefinition = myProcessDefinitionService.GetProcessDefinition(processDefinitionId, dbSession);
                    processInstance = new ProcessInstanceImpl(ActorId, processDefinition);
                    processInstanceRepository.Save(processInstance, dbSession);//到這裏應該存了ProcessInstance,RootFlow

                    ExecutionContext executionContext = new ExecutionContext();
                    //logRepository.CreateLog();
                    processDefinition.StartState.CheckAccess(attributeValues);

                    attributeService = new AttributeService((FlowImpl)processInstance.RootFlow, dbSession);
                    attributeService.StoreAttributeValue(attributeValues);//儲存傳入的欄位值
                    attributeService.StoreRole(organisationService.FindActorById(ActorId), (ActivityStateImpl)processDefinition.StartState);

                    //flow的node推進到下一關卡
                    //flow的actor=解析出來的actor.Id
                    transitionService = new TransitionService(ActorId, dbSession);
                    TransitionImpl transitionTo = transitionService.GetTransition(transitionName, processDefinition.StartState, dbSession);
                    transitionService.ProcessTransition(transitionTo, (FlowImpl)processInstance.RootFlow, dbSession);

                    session.Flush();
                    tran.Commit();
                }
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
                    IActor actor = organisationService.FindActorById(actorId);
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
            try
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    DbSession dbSession = new DbSession(session);
                    FlowImpl flow = flowRepository.GetFlow(flowId,dbSession);
                    ActivityStateImpl activityState = (ActivityStateImpl)flow.Node;

                    ExecutionContext executionContext = new ExecutionContext();
                    activityState.CheckAccess(attributeValues);

                    attributeService = new AttributeService(flow,dbSession);
                    attributeService.StoreAttributeValue(attributeValues);

                    transitionService = new TransitionService(ActorId, dbSession);
                    TransitionImpl transitionTo = transitionService.GetTransition(transitionName, activityState, dbSession);
                    transitionService.ProcessTransition(transitionTo, flow, dbSession);

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
                //ServiceLocator.Instance.Release(organisationComponent);
            }
            return flows;
        }
    }
}
