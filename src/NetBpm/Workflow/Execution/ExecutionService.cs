using System;
using System.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Log.Impl;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;
using NHibernate;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Execution.Impl
{
	public class ExecutionService
	{
		private static readonly ExecutionEngineImpl engine = ExecutionEngineImpl.Instance;
        private static readonly DelegationService delegationService = new DelegationService();
		private static readonly AuthorizationHelper authorizationHelper = AuthorizationHelper.Instance;
		private static readonly ExecutionService instance = new ExecutionService();
        private static readonly ProcessDefinitionRepository definitionRepository = ProcessDefinitionRepository.Instance;
        private static readonly TaskRepository taskRepository = TaskRepository.Instance;
        private static readonly TransitionRepository transitionRepository = TransitionRepository.Instance;
        private static readonly FieldRepository fieldRepository = FieldRepository.Instance;
		private static readonly ILog log = LogManager.GetLogger(typeof (ExecutionService));

		/// <summary> gets the singleton instance.</summary>
		public static ExecutionService Instance
		{
			get { return instance; }
		}

		private ExecutionService()
		{
		}

        //private const String queryFindTasks = "select flow " +
        //    "from flow in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
        //    "where flow.ActorId = ?";

		public IList GetTaskList(String authenticatedActorId, String actorId, Relations relations, DbSession dbSession, IOrganisationService organisationComponent)
		{
			IList tasks = null;
			IActor actor = organisationComponent.FindActorById(actorId);

			if (actor is IUser)
			{
				log.Debug("getting task lists for actor --> User : [" + actor + "]");
                tasks = taskRepository.FindTasks(actorId, dbSession);
			}
			else if (actor is IGroup)
			{
				log.Debug("getting task lists for actor --> Group : [" + actor + "]");
				tasks = GetGroupTaskList(authenticatedActorId, null, actorId, dbSession, organisationComponent);
			}

			if (relations != null)
			{
				relations.Resolve(tasks);
			}

			return tasks;
		}

		public IList GetTaskList(String authenticatedActorId, IList actorIds, Relations relations, DbSession dbSession, IOrganisationService organisationComponent)
		{
			ArrayList tasks = null;
			IEnumerator actorIdsIterator = actorIds.GetEnumerator();
			while (actorIdsIterator.MoveNext())
			{
				String actorId = (String) actorIdsIterator.Current;
				if (tasks == null)
				{
					tasks = new ArrayList();
				}
				tasks.AddRange(GetTaskList(authenticatedActorId, actorId, relations, dbSession, organisationComponent));
			}
			return tasks;
		}

		public IProcessInstance StartProcessInstance(String authenticatedActorId, Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations, DbSession dbSession, IOrganisationService organisationComponent)
		{
			ProcessInstanceImpl processInstance = null;

			// First check if the actor is allowed to start this instance
			authorizationHelper.CheckStartProcessInstance(authenticatedActorId, processDefinitionId, attributeValues, transitionName, dbSession);

			// get the process-definition and its start-state    
            ProcessDefinitionImpl processDefinition = (ProcessDefinitionImpl)definitionRepository.GetProcessDefinition(processDefinitionId, null, dbSession);
			StartStateImpl startState = (StartStateImpl) processDefinition.StartState;

			log.Info("actor '" + authenticatedActorId + "' starts an instance of process '" + processDefinition.Name + "'...");

			processInstance = new ProcessInstanceImpl(authenticatedActorId, processDefinition);
			FlowImpl rootFlow = (FlowImpl) processInstance.RootFlow;

            ExecutionContextImpl executionContext = new ExecutionContextImpl(authenticatedActorId, rootFlow, dbSession, organisationComponent);

			// save the process instance to allow hibernate queries    
			dbSession.Save(processInstance);
			//dbSession.Lock(processInstance,LockMode.Upgrade);

            delegationService.RunActionsForEvent(EventType.BEFORE_PERFORM_OF_ACTIVITY, startState.Id, executionContext,dbSession);

			// store the attributes
			executionContext.CreateLog(authenticatedActorId, EventType.PROCESS_INSTANCE_START);
            //LogImpl logImpl = rootFlow.CreateLog(authenticatedActorId, EventType.PROCESS_INSTANCE_START);//new add
			executionContext.CheckAccess(attributeValues, startState);
            //startState.CheckAccess(attributeValues);
            //看來也找不到AttributeInstance
			executionContext.StoreAttributeValues(attributeValues);

			// if this activity has a role-name, save the actor in the corresponding attribute
			executionContext.StoreRole(authenticatedActorId, startState);

            // run the actions
            delegationService.RunActionsForEvent(EventType.PROCESS_INSTANCE_START, processDefinitionId, executionContext,dbSession);

			// from here on, we consider the actor as being the previous actor
			executionContext.SetActorAsPrevious();

			// process the start-transition
            TransitionImpl startTransition = transitionRepository.GetTransition(transitionName, startState, dbSession);
            engine.ProcessTransition(startTransition, executionContext, dbSession);

            // run the actions
            delegationService.RunActionsForEvent(EventType.AFTER_PERFORM_OF_ACTIVITY, startState.Id, executionContext,dbSession);

			// flush the updates to the db
			dbSession.Update(processInstance);
			dbSession.Flush();

			//@portme
/*			if (relations != null)
			{
				relations.resolve(processInstance);
			}
*/
			return processInstance;
		}

        //private const String queryFieldsByState = "select f from f in class NetBpm.Workflow.Definition.Impl.FieldImpl " +
        //    "where f.State.Id = ? " +
        //    "order by f.Index";

		public IActivityForm GetStartForm(String authenticatedActorId, Int64 processDefinitionId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			IActivityForm activityForm = null;

			// First check if the actor is allowed to get this form
			authorizationHelper.CheckGetStartForm(authenticatedActorId, processDefinitionId, dbSession);

			ProcessDefinitionImpl processDefinition = (ProcessDefinitionImpl) dbSession.Load(typeof (ProcessDefinitionImpl), processDefinitionId);
			StartStateImpl startState = (StartStateImpl) processDefinition.StartState;

			// create a convenient map from the attribute-names to the fields
            IList fields = fieldRepository.FindFieldsByState(startState.Id, dbSession);
			IDictionary attributeValues = new Hashtable();
			IEnumerator iter = fields.GetEnumerator();
			while (iter.MoveNext())
			{
				FieldImpl field = (FieldImpl) iter.Current;

				// if the attribute has an initial value
				AttributeImpl attribute = (AttributeImpl) field.Attribute;
				String attributeName = attribute.Name;
				String initialValue = attribute.InitialValue;
				if ((Object) initialValue != null && (FieldAccessHelper.IsReadable(field.Access) || FieldAccessHelper.IsWritable(field.Access)))
				{
					// start form contains only fields that are readable or writable        

					// get it and store it in the attributeValues
					AttributeInstanceImpl attributeInstance = new AttributeInstanceImpl();
					attributeInstance.Attribute = attribute;
					attributeInstance.ValueText = initialValue;
					attributeValues[attributeName] = attributeInstance.GetValue();
				}
			}

			activityForm = new ActivityFormImpl(processDefinition, fields, attributeValues);

			return activityForm;
		}

		public IActivityForm GetActivityForm(String authenticatedActorId, Int64 flowId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			IActivityForm activityForm = null;

			// First check if the actor is allowed to get this form
			authorizationHelper.CheckGetActivityForm(authenticatedActorId, flowId, dbSession);

			FlowImpl flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			StateImpl state = (StateImpl) flow.Node;

			// create an executionContext for easy attributeValue retrieval    
			ExecutionContextImpl executionContext = new ExecutionContextImpl(null, flow, dbSession, organisationComponent);

			// create a convenient map from the attribute-names to the fields
			IList fields = fieldRepository.FindFieldsByState(state.Id,dbSession);
			IDictionary attributeValues = new Hashtable();
			IEnumerator iter = fields.GetEnumerator();
			while (iter.MoveNext())
			{
				FieldImpl field = (FieldImpl) iter.Current;
				if (FieldAccessHelper.IsReadable(field.Access) || FieldAccessHelper.IsWritable(field.Access))
				{
					// activity form contains only readable or writeable fields
					String attributeName = field.Attribute.Name;
					if (executionContext.GetAttribute(attributeName) != null)
					{
						// attribute might not exist (this will cause a warning already being logged previusly)
						attributeValues[attributeName] = executionContext.GetAttribute(attributeName);
					}
				}
			}

			activityForm = new ActivityFormImpl(flow, fields, attributeValues);

			return activityForm;
		}

		public void SaveActivity(String authenticatedActorId, Int64 flowId, IDictionary attributeValues, DbSession dbSession, IOrganisationService organisationComponent)
		{
			// get the flow
			FlowImpl flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			// create the execution-context
			ExecutionContextImpl executionContext = new ExecutionContextImpl(authenticatedActorId, flow, dbSession, organisationComponent);
			executionContext.StoreAttributeValues(attributeValues);
		}

		public IList PerformActivity(String authenticatedActorId, Int64 flowId, IDictionary attributeValues, String transitionName, Relations relations, DbSession dbSession, IOrganisationService organisationComponent)
		{
			IList assignedFlows = null;
			// get the flow
			FlowImpl flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			dbSession.Lock(flow.ProcessInstance, LockMode.Upgrade);
			ActivityStateImpl activityState = (ActivityStateImpl) flow.Node;

			// TODO : check which part can move to the DefaultAuthorizationHandler
			if ((Object) flow.ActorId == null)
			{
				throw new SystemException("the flow on which you try to perform an activity is not assigned to an actor");
			}
			else
			{
				if ((Object) authenticatedActorId == null)
				{
					throw new AuthorizationException("you can't perform an activity because you are not authenticated");
				}
				//		else if ( ! authenticatedActorId.equals( flow.getActorId() ) ) {
				//        throw new AuthorizationException( "activity '" + activityState.getName() + "' in flow " + flow.getId() + " is not assigned to the authenticated actor (" + authenticatedActorId + ") but to " + flow.getActorId() );
				//      }
			}

			// first check if the actor is allowed to perform this activity
			authorizationHelper.CheckPerformActivity(authenticatedActorId, flowId, attributeValues, transitionName, dbSession);

			log.Info("actor '" + authenticatedActorId + "' performs activity '" + activityState.Name + "'...");

			// create the execution-context
			ExecutionContextImpl executionContext = new ExecutionContextImpl(authenticatedActorId, flow, dbSession, organisationComponent);

			// if this activity has a role-name, save the actor in the corresponding attribute
			// attributeValues = state.addRoleAttributeValue( attributeValues, authenticatedActorId, organisationComponent );

			// log event & trigger actions 
            delegationService.RunActionsForEvent(EventType.BEFORE_PERFORM_OF_ACTIVITY, activityState.Id, executionContext,dbSession);

			// store the supplied attribute values
			executionContext.CreateLog(authenticatedActorId, EventType.PERFORM_OF_ACTIVITY);
			executionContext.AddLogDetail(new ObjectReferenceImpl(activityState));
			executionContext.CheckAccess(attributeValues, activityState);
			executionContext.StoreAttributeValues(attributeValues);

			// log event & trigger actions 
            delegationService.RunActionsForEvent(EventType.PERFORM_OF_ACTIVITY, activityState.Id, executionContext,dbSession);

			// from here on, we consider the actor as being the previous actor
            //因為繼續往下跑，ActorId就有可能轉換成下一關卡的處理人員
            //所以previousActorId就是現在的登入人員
			executionContext.SetActorAsPrevious();

			// select and process the transition
            TransitionImpl startTransition = transitionRepository.GetTransition(transitionName, activityState, dbSession);
            engine.ProcessTransition(startTransition, executionContext, dbSession);

			// log event & trigger actions 
            delegationService.RunActionsForEvent(EventType.AFTER_PERFORM_OF_ACTIVITY, activityState.Id, executionContext,dbSession);

			assignedFlows = executionContext.AssignedFlows;

			// flush the updates to the db
			dbSession.Update(flow.ProcessInstance);
			dbSession.Flush();

			if (relations != null)
			{
				relations.Resolve(assignedFlows);
			}
			dbSession.Update(flow.ProcessInstance);
			return assignedFlows;
		}

		//@todo delete parameter organisationComponent
		public void DelegateActivity(String authenticatedActorId, Int64 flowId, String delegateActorId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			// first check if the actor is allowed to delegate this activity
			authorizationHelper.CheckDelegateActivity(authenticatedActorId, flowId, delegateActorId, dbSession);

			// reassign the flow
			FlowImpl flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			flow.ActorId = delegateActorId;

			// flush the updates to the db
			dbSession.Update(flow);
			dbSession.Flush();
		}

		public void CancelProcessInstance(String authenticatedActorId, Int64 processInstanceId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			// first check if the actor is allowed to cancel this process instance
			authorizationHelper.CheckCancelProcessInstance(authenticatedActorId, processInstanceId, dbSession);

			ProcessInstanceImpl processInstance = (ProcessInstanceImpl) dbSession.Load(typeof (ProcessInstanceImpl), processInstanceId);

			log.Info("actor '" + authenticatedActorId + "' cancels processInstance '" + processInstanceId + "'...");

			if (!processInstance.EndHasValue)
			{
				CancelFlowRecursive((FlowImpl) processInstance.RootFlow, DateTime.Now);
				ExecutionContextImpl executionContext = new ExecutionContextImpl(authenticatedActorId, (FlowImpl) processInstance.RootFlow, dbSession, organisationComponent);
				executionContext.CreateLog(authenticatedActorId, EventType.PROCESS_INSTANCE_CANCEL);
				EndStateImpl endState = (EndStateImpl) processInstance.ProcessDefinition.EndState;
                engine.ProcessEndState(endState, executionContext, dbSession);
				processInstance.End = DateTime.Now;

				// flush the updates to the db
				dbSession.Update(processInstance);
				dbSession.Flush();
			}
			else
			{
				throw new SystemException("couldn't cancel process instance : process instance '" + processInstanceId + "' was already finished");
			}
		}

		public void CancelFlow(String authenticatedActorId, Int64 flowId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			// first check if the actor is allowed to cancel this flow
			authorizationHelper.CheckCancelFlow(authenticatedActorId, flowId, dbSession);

			FlowImpl flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);

			log.Info("actor '" + authenticatedActorId + "' cancels flow '" + flowId + "'...");

			// only perform the cancel if this flow is not finished yet
			if (!flow.EndHasValue)
			{
				ExecutionContextImpl executionContext = new ExecutionContextImpl(authenticatedActorId, flow, dbSession, organisationComponent);
				executionContext.CreateLog(authenticatedActorId, EventType.FLOW_CANCEL);

				if (flow.IsRootFlow())
				{
					// set the flow in the end-state
					log.Debug("setting root flow to the end state...");
					EndStateImpl endState = (EndStateImpl) flow.ProcessInstance.ProcessDefinition.EndState;
                    engine.ProcessEndState(endState, executionContext, dbSession);
				}
				else
				{
					// set the flow in the join
					ConcurrentBlockImpl concurrentBlock = (ConcurrentBlockImpl) flow.Node.ProcessBlock;
					JoinImpl join = (JoinImpl) concurrentBlock.Join;
					log.Debug("setting concurrent flow to join '" + join + "'");
                    engine.ProcessJoin(join, executionContext, dbSession);
				}

				// flush the updates to the db
				dbSession.Update(flow);
				dbSession.Flush();
			}
		}

		public IFlow GetFlow(String authenticatedActorId, Int64 flowId, Relations relations, DbSession dbSession)
		{
			// first check if the actor is allowed to get this flow
			authorizationHelper.CheckGetFlow(authenticatedActorId, flowId, dbSession);

			FlowImpl flow = null;
			flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);

			if (relations != null)
			{
				relations.Resolve(flow);
			}

			return flow;
		}

		private void CancelFlowRecursive(FlowImpl flow, DateTime now)
		{
			flow.End = now;
			flow.ActorId = null;
			IEnumerator iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				CancelFlowRecursive((FlowImpl) iter.Current, now);
			}
		}

		private IList GetGroupTaskList(String authenticatedActorId, ArrayList groupTaskLists, String groupId, DbSession dbSession, IOrganisationService organisationComponent)
		{
			if (groupTaskLists == null)
			{
				groupTaskLists = new ArrayList();
			}
			IGroup g = organisationComponent.FindGroupById(groupId, new Relations("parent"));
			IGroup gParent = g.Parent;
			if (gParent != null)
			{
				// scan if this group has more parent(s)
				GetGroupTaskList(authenticatedActorId, groupTaskLists, gParent.Id, dbSession, organisationComponent);
			}
			// no more parent
            IList gTaskLists = taskRepository.FindTasks(g.Id, dbSession);
			groupTaskLists.AddRange(gTaskLists);
			log.Debug("added task lists [" + gTaskLists + "] for group [" + g + "]");

			return groupTaskLists;
		}
	}
}