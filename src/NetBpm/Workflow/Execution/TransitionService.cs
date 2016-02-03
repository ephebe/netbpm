using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class TransitionService
    {
        private TransitionRepository transitionRepository = TransitionRepository.Instance;
        private NHibernate.ISession session;
        private DelegationService delegationService = null;

        public TransitionService(NHibernate.ISession session)
        {
            // TODO: Complete member initialization
            this.session = session;
        }

        public TransitionImpl GetTransition(string transitionName, IState state, DbSession dbSession)
        {
            return transitionRepository.GetTransition(transitionName, (StateImpl)state, dbSession);
        }

        public void ProcessTransition(TransitionImpl transition, FlowImpl flow, DbSession nhSession)
        {
            NodeImpl destination = (NodeImpl)transition.To;
            flow.Node = destination;

            if (destination is ActivityStateImpl)
            {
                ProcessActivityState((ActivityStateImpl)destination, flow, nhSession);
            }
            else if (destination is ProcessStateImpl)
            {
                //ProcessProcessState((ProcessStateImpl)destination, executionContext, dbSession);
            }
            else if (destination is DecisionImpl)
            {
                //ProcessDecision((DecisionImpl)destination, executionContext, dbSession);
            }
            else if (destination is ForkImpl)
            {
                //ProcessFork((ForkImpl)destination, executionContext, dbSession);
            }
            else if (destination is JoinImpl)
            {
                //ProcessJoin((JoinImpl)destination, executionContext, dbSession);
            }
            else if (destination is EndStateImpl)
            {
                //ProcessEndState((EndStateImpl)destination, executionContext, dbSession);
            }
            else
            {
                throw new SystemException("");
            }
        }

        public void ProcessActivityState(ActivityStateImpl activityState, FlowImpl flow, DbSession dbSession)
        {
            String actorId = null;
            String role = activityState.ActorRoleName;
            delegationService = new DelegationService();
            DelegationImpl assignmentDelegation = activityState.AssignmentDelegation;

            if (assignmentDelegation != null)
            {
                // delegate the assignment of the activity-state
                actorId = delegationService.DelegateAssignment(activityState.AssignmentDelegation);
                if ((Object)actorId == null)
                {
                    throw new SystemException("invalid process definition : assigner of activity-state '" + activityState.Name + "' returned null instead of a valid actorId");
                }
            }
            else
            {
                if ((Object)role != null)
                {
                    IActor actor = (IActor)this.GetAttribute(role);
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

        public virtual Object GetAttribute(String name)
        {
            //AttributeInstanceImpl attributeInstance = FindAttributeInstanceInScope(name);
            //if (attributeInstance != null)
            //{
            //    // attribute might not be available (a warning should have been logged previosly)
            //    return attributeInstance.GetValue();
            //}
            return null;
        }
    }
}
