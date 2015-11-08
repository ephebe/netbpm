using System;
using System.Collections;
using Iesi.Collections;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Log.Impl;
using NetBpm.Workflow.Organisation;
using NHibernate.Type;

namespace NetBpm.Workflow.Execution.Impl
{
	public class ExecutionEngineImpl
	{
		private static readonly DelegationHelper delegationHelper = DelegationHelper.Instance;
		private static readonly ExecutionEngineImpl instance = new ExecutionEngineImpl();
		private static readonly ActorExpressionResolver actorExpressionResolver = ActorExpressionResolver.Instance;
		private static readonly ILog log = LogManager.GetLogger(typeof (ExecutionEngineImpl));

		/// <summary> gets the singleton instance.</summary>
		public static ExecutionEngineImpl Instance
		{
			get { return instance; }
		}

		private ExecutionEngineImpl()
		{
		}

		private const String queryFindActionsByEventType = "select a from a in class NetBpm.Workflow.Definition.Impl.ActionImpl " +
			"where a.EventType = ? " +
			"  and a.DefinitionObjectId = ? ";

		public void RunActionsForEvent(EventType eventType, Int64 definitionObjectId, ExecutionContextImpl executionContext)
		{
			log.Debug("processing '" + eventType + "' events for executionContext " + executionContext);

			DbSession dbSession = executionContext.DbSession;

			// find all actions for definitionObject on the given eventType
			Object[] values = new Object[] {eventType, definitionObjectId};
			IType[] types = new IType[] {DbType.INTEGER, DbType.LONG};

			IList actions = dbSession.Find(queryFindActionsByEventType, values, types);
			IEnumerator iter = actions.GetEnumerator();
			log.Debug("list" + actions);
			while (iter.MoveNext())
			{
				ActionImpl action = (ActionImpl) iter.Current;
				log.Debug("action: " + action);
				delegationHelper.DelegateAction(action.ActionDelegation, executionContext);
			}
			log.Debug("ende runActionsForEvent!");
		}

		public void ProcessTransition(TransitionImpl transition, ExecutionContextImpl executionContext)
		{
			log.Debug("processing transition '" + transition + "' for flow '" + executionContext.GetFlow() + "'");

			// trigger all the actions scheduled for this transition
			RunActionsForEvent(EventType.TRANSITION, transition.Id, executionContext);

			// first set the state of the execution context and the flow
			// to the node that is going to be processed 
			FlowImpl flow = (FlowImpl) executionContext.GetFlow();
			NodeImpl destination = (NodeImpl) transition.To;
			flow.Node = destination;
			executionContext.SetNode(destination);

			// note : I want to keep the engine methods grouped in this class, that is why I
			// didn't use inheritance but used an instanceof-switch instead.
            
			if (destination is ActivityStateImpl)
			{
				ProcessActivityState((ActivityStateImpl) destination, executionContext);
			}
			else if (destination is ProcessStateImpl)
			{
				ProcessProcessState((ProcessStateImpl) destination, executionContext);
			}
			else if (destination is DecisionImpl)
			{
				ProcessDecision((DecisionImpl) destination, executionContext);
			}
			else if (destination is ForkImpl)
			{
				ProcessFork((ForkImpl) destination, executionContext);
			}
			else if (destination is JoinImpl)
			{
				ProcessJoin((JoinImpl) destination, executionContext);
			}
			else if (destination is EndStateImpl)
			{
				ProcessEndState((EndStateImpl) destination, executionContext);
			}
			else
			{
				throw new SystemException("");
			}
		}

		public void ProcessActivityState(ActivityStateImpl activityState, ExecutionContextImpl executionContext)
		{
			// first set the flow-state to the activity-state  
			FlowImpl flow = (FlowImpl) executionContext.GetFlow();

			log.Debug("processing activity-state '" + activityState + "' for flow '" + executionContext.GetFlow() + "'");

			// execute the actions scheduled for this assignment
			RunActionsForEvent(EventType.BEFORE_ACTIVITYSTATE_ASSIGNMENT, activityState.Id, executionContext);

			String actorId = null;
			String role = activityState.ActorRoleName;
			DelegationImpl assignmentDelegation = activityState.AssignmentDelegation;

			if (assignmentDelegation != null)
			{
				// delegate the assignment of the activity-state
				actorId = delegationHelper.DelegateAssignment(activityState.AssignmentDelegation, executionContext);
				if ((Object) actorId == null)
				{
					throw new SystemException("invalid process definition : assigner of activity-state '" + activityState.Name + "' returned null instead of a valid actorId");
				}
				log.Debug("setting actor of flow " + flow + " to " + actorId);
			}
			else
			{
				// get the assigned actor from the specified attribute instance
				if ((Object) role != null)
				{
					IActor actor = (IActor) executionContext.GetAttribute(role);
					if (actor == null)
					{
						throw new SystemException("invalid process definition : activity-state must be assigned to role '" + role + "' but that attribute instance is null");
					}
					actorId = actor.Id;
				}
				else
				{
					throw new SystemException("invalid process definition : activity-state '" + activityState.Name + "' does not have an assigner or a role");
				}
			}

			flow.ActorId = actorId;

			// If necessary, store the actor in the role
			if (((Object) role != null) && (assignmentDelegation != null))
			{
				executionContext.StoreRole(actorId, activityState);
			}

			// the client of performActivity wants to be Informed of the people in charge of the process
			executionContext.AssignedFlows.Add(flow);

			// log the assignment
			executionContext.CreateLog(actorId, EventType.AFTER_ACTIVITYSTATE_ASSIGNMENT);
			executionContext.AddLogDetail(new ObjectReferenceImpl(activityState));

			// execute the actions scheduled for this assignment
			RunActionsForEvent(EventType.AFTER_ACTIVITYSTATE_ASSIGNMENT, activityState.Id, executionContext);
		}

		public void ProcessProcessState(ProcessStateImpl processState, ExecutionContextImpl executionContext)
		{
			// TODO : try to group similarities between this method and ExecutionComponentImpl.startProcessInstance and 
			//        group them in a common method 

			// provide a convenient local var for the database session
			DbSession dbSession = executionContext.DbSession;

			// get the sub-process-definition and its start-state    
			ProcessDefinitionImpl subProcessDefinition = (ProcessDefinitionImpl) processState.SubProcess;
			StartStateImpl startState = (StartStateImpl) subProcessDefinition.StartState;

			log.Info("processState '" + processState.Name + "' starts an instance of process '" + subProcessDefinition.Name + "'...");

			// get the actor that is supposed to start this process instance
			IActor subProcessStarter = actorExpressionResolver.ResolveArgument(processState.ActorExpression, executionContext);
			String subProcessStarterId = subProcessStarter.Id;

			// create the process-instance
			ProcessInstanceImpl subProcessInstance = new ProcessInstanceImpl(subProcessStarterId, subProcessDefinition);
			FlowImpl rootFlow = (FlowImpl) subProcessInstance.RootFlow;

			// attach the subProcesInstance to the parentFlow
			FlowImpl superProcessFlow = (FlowImpl) executionContext.GetFlow();
			superProcessFlow.SetSubProcessInstance(subProcessInstance);
			subProcessInstance.SuperProcessFlow = superProcessFlow;

			// create the execution context for the sub-process 
			ExecutionContextImpl subExecutionContext = new ExecutionContextImpl(subProcessStarterId, rootFlow, dbSession, executionContext.GetOrganisationComponent());

			// save the process instance to allow hibernate queries    
			dbSession.Save(subProcessInstance);

			// add the log
			executionContext.CreateLog(EventType.SUB_PROCESS_INSTANCE_START);
			executionContext.AddLogDetail(new ObjectReferenceImpl(subProcessInstance));

			// delegate the attributeValues
			Object[] processInvocationData = delegationHelper.DelegateProcessInvocation(processState.ProcessInvokerDelegation, subExecutionContext);
			String transitionName = (String) processInvocationData[0];
			IDictionary attributeValues = (IDictionary) processInvocationData[1];

			// store the attributes
			subExecutionContext.CreateLog(subProcessStarterId, EventType.PROCESS_INSTANCE_START);
			subExecutionContext.StoreAttributeValues(attributeValues);
			subExecutionContext.StoreRole(subProcessStarterId, startState);

			// log event & trigger actions 
			RunActionsForEvent(EventType.SUB_PROCESS_INSTANCE_START, processState.Id, subExecutionContext);
			RunActionsForEvent(EventType.PROCESS_INSTANCE_START, subProcessDefinition.Id, subExecutionContext);

			// from here on, we consider the actor as being the previous actor
			subExecutionContext.SetActorAsPrevious();

			// process the start-transition
			TransitionImpl startTransition = subExecutionContext.GetTransition(transitionName, startState, dbSession);
			ProcessTransition(startTransition, subExecutionContext);

			// add the assigned flows of the subContext to the parentContext
			executionContext.AssignedFlows.AddRange(subExecutionContext.AssignedFlows);

			// flush the updates to the db
			dbSession.Update(subProcessInstance);
			dbSession.Flush();
		}

		public void ProcessDecision(DecisionImpl decision, ExecutionContextImpl executionContext)
		{
			// trigger actions, scheduled before the decision actually is made 
			RunActionsForEvent(EventType.BEFORE_DECISION, decision.Id, executionContext);

			// delegate the decision 
			TransitionImpl selectedTransition = delegationHelper.DelegateDecision(decision.DecisionDelegation, executionContext);

			// process the selected transition
			ProcessTransition(selectedTransition, executionContext);

			// trigger actions, scheduled after the decision is made 
			RunActionsForEvent(EventType.AFTER_DECISION, decision.Id, executionContext);
		}

		public void ProcessFork(ForkImpl fork, ExecutionContextImpl executionContext)
		{
			log.Debug("forking flow " + executionContext.GetFlow());

			// First initialize the children of the flow to be forked
			FlowImpl flow = (FlowImpl) executionContext.GetFlow();
			flow.Children = new ListSet();

			// Then initialise the forked flows in the execution context
			executionContext.ForkedFlows = new ArrayList();

			DelegationImpl delegation = fork.ForkDelegation;
			if (delegation != null)
			{
				delegationHelper.DelegateFork(fork.ForkDelegation, executionContext);
			}
			else
			{
				// execute the default fork behaviour
				IEnumerator iter = fork.LeavingTransitions.GetEnumerator();
				while (iter.MoveNext())
				{
					TransitionImpl transition = (TransitionImpl) iter.Current;
					executionContext.ForkFlow(transition, null);
				}
			}

			// create the fork event & remember the parent flow
			FlowImpl parentFlow = (FlowImpl) executionContext.GetFlow();
			executionContext.CreateLog(EventType.FORK);

			// log the event
			executionContext.SetFlow(parentFlow);
			IList forkedFlows = executionContext.ForkedFlows;
			IEnumerator iter2 = forkedFlows.GetEnumerator();
			while (iter2.MoveNext())
			{
				ForkedFlow forkedFlow = (ForkedFlow) iter2.Current;
				log.Debug("adding object reference [" + forkedFlow.Flow + "] to flow [" + parentFlow + "]");
				executionContext.AddLogDetail(new ObjectReferenceImpl(forkedFlow.Flow));
			}

			// loop over all flows that were forked in the ForkHandler implementation
			iter2 = forkedFlows.GetEnumerator();
			while (iter2.MoveNext())
			{
				ForkedFlow forkedFlow = (ForkedFlow) iter2.Current;

				// trigger actions, scheduled after the creation and setting of the attributeValues
				// but before the fork is being processed
				RunActionsForEvent(EventType.FORK, fork.Id, executionContext);

				// then process the forked flow transition
				executionContext.SetFlow(forkedFlow.Flow);
				ProcessTransition(forkedFlow.Transition, executionContext);
			}
		}

		public void ProcessJoin(JoinImpl join, ExecutionContextImpl executionContext)
		{
			// First set the state of the flow to finished
			FlowImpl joiningFlow = (FlowImpl) executionContext.GetFlow();
			joiningFlow.End = DateTime.Now;
			joiningFlow.ActorId = null;
			joiningFlow.Node = join; // setting the node is not necessary if this method is called
			// from processTransition, but it is necessary if this method is
			// called from cancelFlow in the component-impl.

			// if parent-reactivation of the flow is true, this means that the parent-flow
			// not yet has been reactivated.  In that case we have to see if it needs to be 
			// reactivated.  In the other case (parent-reactivation is false) we don't
			// need to do anything because this means that the parent-flow was already 
			// reactivated before.  
			if (!false.Equals(joiningFlow.ParentReactivation))
			{
				// check if the parent needs to be reactivated    
				bool parentReactivation = false;
				IList concurrentFlows = executionContext.GetOtherActiveConcurrentFlows();
				if (concurrentFlows.Count == 0)
				{
					// if no concurrent flows are present any more, reactivation is forced 
					parentReactivation = true;
				}
				else
				{
					// if other concurrent flows are present, the decision to reactivate is left 
					// to the join-delegation (if there is one specified)
					DelegationImpl joinDelegation = join.JoinDelegation;
					// if no joinDelegation was specified, parentReactivation remains false
					// so the behaviour is like an and-join. (=sunchronizing merge)
					if (joinDelegation != null)
					{
						parentReactivation = delegationHelper.DelegateJoin(join.JoinDelegation, executionContext);
					}
				}

				if (parentReactivation)
				{
					// make sure the other concurrent flows will not reactivate the
					// parent again
					IEnumerator iter = concurrentFlows.GetEnumerator();
					while (iter.MoveNext())
					{
						//UPGRADE_TODO: Methode "java.util.Iterator.next" wurde in 'IEnumerator.Current' konvertiert und weist ein anderes Verhalten auf. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						FlowImpl concurrentFlow = (FlowImpl) iter.Current;
						concurrentFlow.ParentReactivation = false;
					}

					// reactivate the parent by first setting the parentflow into the executionContext
					FlowImpl parentFlow = (FlowImpl) joiningFlow.Parent;
					executionContext.SetFlow(parentFlow);
					// and then process the (single, checked at process-archive-parsing-time) leaving transition. 
					ISet leavingTransitions = join.LeavingTransitions;
					iter = leavingTransitions.GetEnumerator();
					if (iter.MoveNext())
					{
						TransitionImpl leavingTransition = (TransitionImpl) iter.Current;
						ProcessTransition(leavingTransition, executionContext);
					} else {
						// no transition throw exception?
					}
				}
			}
		}

		public void ProcessEndState(EndStateImpl endState, ExecutionContextImpl executionContext)
		{
			RunActionsForEvent(EventType.PROCESS_INSTANCE_END, endState.ProcessDefinition.Id, executionContext);
			executionContext.CreateLog(EventType.PROCESS_INSTANCE_END);

			FlowImpl rootFlow = (FlowImpl) executionContext.GetFlow();
			rootFlow.ActorId = null;
			rootFlow.End = DateTime.Now;
			rootFlow.Node = endState; // setting the node is not necessary if this method is called
			// from processTransition, but it is necessary if this method is
			// called from cancelProcessInstance in the component-impl.

			ProcessInstanceImpl processInstance = (ProcessInstanceImpl) executionContext.GetProcessInstance();
			FlowImpl superProcessFlow = (FlowImpl) processInstance.SuperProcessFlow;
			if (superProcessFlow != null)
			{
				log.Debug("reactivating the super process...");

				// create the execution context for the parent-process 
				ExecutionContextImpl superExecutionContext = new ExecutionContextImpl(executionContext.PreviousActorId, superProcessFlow, executionContext.DbSession, executionContext.GetOrganisationComponent());
				superExecutionContext.SetInvokedProcessContext(executionContext);

				// delegate the attributeValues
				ProcessStateImpl processState = (ProcessStateImpl) superProcessFlow.Node;
				Object[] completionData = delegationHelper.DelegateProcessTermination(processState.ProcessInvokerDelegation, superExecutionContext);
				IDictionary attributeValues = (IDictionary) completionData[0];
				String transitionName = (String) completionData[1];
				TransitionImpl transition = superExecutionContext.GetTransition(transitionName, processState, executionContext.DbSession);

				// process the super process transition
				ProcessTransition(transition, superExecutionContext);
			}
		}
	}
}