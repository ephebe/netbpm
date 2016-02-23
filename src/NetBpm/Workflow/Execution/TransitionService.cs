using Iesi.Collections;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Delegation.Impl.Decision;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class TransitionService
    {
        private TransitionRepository transitionRepository = TransitionRepository.Instance;
        private AttributeRepository attributeRepository = AttributeRepository.Instance;
        private FlowRepository flowRepository = FlowRepository.Instance;
        private DbSession session;
        private DelegationService delegationService = null;
        private ActorExpressionResolverService actorExpressionResolverService = null;

        public TransitionService(string previousActorId,DbSession session)
        {
            // TODO: Complete member initialization
            this.session = session;
            delegationService = new DelegationService();
            actorExpressionResolverService = new ActorExpressionResolverService(previousActorId);
        }

        public TransitionImpl GetTransition(string transitionName, INode node, DbSession dbSession)
        {
            return transitionRepository.GetTransition(transitionName, node, dbSession);
        }

        public void ProcessTransition(TransitionImpl transition, FlowImpl flow, DbSession dbSession)
        {
            NodeImpl destination = (NodeImpl)transition.To;
            flow.Node = destination;

            if (destination is ActivityStateImpl)
            {
                ProcessActivityState((ActivityStateImpl)destination, flow, dbSession);
            }
            else if (destination is ProcessStateImpl)
            {
                //ProcessProcessState((ProcessStateImpl)destination, executionContext, dbSession);
            }
            else if (destination is DecisionImpl)
            {
                ProcessDecision((DecisionImpl)destination, flow, dbSession);
            }
            else if (destination is ForkImpl)
            {
                ProcessFork((ForkImpl)destination, flow, dbSession);
            }
            else if (destination is JoinImpl)
            {
                ProcessJoin((JoinImpl)destination, flow, dbSession);
            }
            else if (destination is EndStateImpl)
            {
                ProcessEndState((EndStateImpl)destination, flow,dbSession);
            }
            else
            {
                throw new SystemException("");
            }
        }

        public void ProcessJoin(JoinImpl join, FlowImpl joiningFlow, DbSession dbSession)
        {
            joiningFlow.End = DateTime.Now;
            joiningFlow.ActorId = null;
            joiningFlow.Node = join;

            if (false != joiningFlow.ParentReactivation)
            {  
                bool parentReactivation = false;
                IList concurrentFlows = flowRepository.GetOtherActiveConcurrentFlows(joiningFlow.Id, dbSession);
                if (concurrentFlows.Count == 0)
                {
                    parentReactivation = true;
                }
                else
                {

                    //DelegationImpl joinDelegation = join.JoinDelegation;

                    //if (joinDelegation != null)
                    //{
                    //    IJoinHandler joiner = (IJoinHandler)joinDelegation.GetDelegate();
                    //    IDictionary attributes = joinDelegation.ParseConfiguration();
                    //    parentReactivation = delegationHelper.DelegateJoin(join.JoinDelegation, executionContext);
                    //}
                }

                if (parentReactivation)
                {
                    IEnumerator iter = concurrentFlows.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        FlowImpl concurrentFlow = (FlowImpl)iter.Current;
                        concurrentFlow.ParentReactivation = false;
                    }

                    // reactivate the parent by first setting the parentflow into the executionContext
                    FlowImpl parentFlow = (FlowImpl)joiningFlow.Parent;
                    //executionContext.SetFlow(parentFlow);
                    // and then process the (single, checked at process-archive-parsing-time) leaving transition. 
                    ISet leavingTransitions = join.LeavingTransitions;
                    iter = leavingTransitions.GetEnumerator();
                    if (iter.MoveNext())
                    {
                        TransitionImpl leavingTransition = (TransitionImpl)iter.Current;
                        ProcessTransition(leavingTransition, parentFlow, dbSession);
                    }
                    else
                    {
                        // no transition throw exception?
                    }
                }
            }
        }

        public void ProcessActivityState(ActivityStateImpl activityState, FlowImpl flow, DbSession dbSession)
        {
            String actorId = null;
            String role = activityState.ActorRoleName;
          
            DelegationImpl assignmentDelegation = activityState.AssignmentDelegation;

            if (assignmentDelegation != null)
            {
                var delegateParameters = activityState.AssignmentDelegation.ParseConfiguration();
                actorExpressionResolverService.CurrentScope = flow;
                actorExpressionResolverService.DbSession = dbSession;
                actorId = actorExpressionResolverService.ResolveArgument(delegateParameters["expression"].ToString()).Id;

                if ((Object)actorId == null)
                {
                    throw new SystemException("invalid process definition : assigner of activity-state '" + activityState.Name + "' returned null instead of a valid actorId");
                }
            }
            else
            {
                if ((Object)role != null)
                {
                    IActor actor = null;
                    var attr = attributeRepository.FindAttributeInstanceInScope(role, flow, dbSession);
                    if (attr != null)
                        actor = (IActor)attr.GetValue();

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
            if ((string.IsNullOrEmpty(role) == false) && (assignmentDelegation != null))
            {
                //executionContext.StoreRole(actorId, activityState);
            }

            // the client of performActivity wants to be Informed of the people in charge of the process
            //executionContext.AssignedFlows.Add(flow);
        }

        private void ProcessEndState(EndStateImpl endState, FlowImpl flow,DbSession dbSession)
        {
            flow.ActorId = null;
            flow.End = DateTime.Now;
            flow.Node = endState; 
        }

        public void ProcessDecision(DecisionImpl decision, FlowImpl flow, DbSession dbSession)
        {
            //var delegateParameters = delegationService.ParseConfiguration(decision.DecisionDelegation);

            //IDecisionHandler decisionHandler = (IDecisionHandler)delegationService.GetDelegate(decision.DecisionDelegation);
            var attributes = decision.DecisionDelegation.ParseConfiguration();
            string transiationName = new EvaluationDecision().Decide(attributes["attribute"].ToString());
            TransitionImpl selectedTransition = this.GetTransition(transiationName, decision, dbSession);
            //// delegate the decision 
            //TransitionImpl selectedTransition = delegationHelper.DelegateDecision(decision.DecisionDelegation, executionContext);

            //// process the selected transition
            ProcessTransition(selectedTransition, flow, dbSession);
        }

        public void ProcessFork(ForkImpl fork, FlowImpl flow, DbSession dbSession)
        {
            // First initialize the children of the flow to be forked
            flow.Children = new ListSet();

            DelegationImpl delegation = fork.ForkDelegation;
            IList<ForkedFlow> forkedFlows = new List<ForkedFlow>();
            if (delegation != null)
            {
                //delegationHelper.DelegateFork(fork.ForkDelegation, executionContext);
            }
            else
            {
                // execute the default fork behaviour
                IEnumerator iter = fork.LeavingTransitions.GetEnumerator();
                while (iter.MoveNext())
                {
                    TransitionImpl transition = (TransitionImpl)iter.Current;
                    forkedFlows.Add(this.ForkFlow(transition, flow,null));
                }
            }


            IEnumerator iter2 = forkedFlows.GetEnumerator();
            while (iter2.MoveNext())
            {
                ForkedFlow forkedFlow = (ForkedFlow)iter2.Current;
            }

            // loop over all flows that were forked in the ForkHandler implementation
            iter2 = forkedFlows.GetEnumerator();
            while (iter2.MoveNext())
            {
                ForkedFlow forkedFlow = (ForkedFlow)iter2.Current;

                ProcessTransition(forkedFlow.Transition, forkedFlow.Flow, dbSession);
            }
        }

        public ForkedFlow ForkFlow(TransitionImpl transition, FlowImpl parentFlow, IDictionary attributeValues)
        {
            // create the subflow 
            FlowImpl subFlow = new FlowImpl(transition.Name, parentFlow, (ProcessBlockImpl)transition.From.ProcessBlock);
            parentFlow.Children.Add(subFlow);

            // save it 
            //_dbSession.SaveOrUpdate(subFlow);

            // add the transition and the flow to the set of created sub-flows
            return new ForkedFlow(transition, subFlow);
        }
    }
}
