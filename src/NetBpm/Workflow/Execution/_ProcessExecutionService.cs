using NetBpm.Util.DB;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class ProcessExecutionService
    {
        private static readonly DelegationHelper delegationHelper = DelegationHelper.Instance;

        public ProcessInstanceImpl Start(ProcessDefinitionImpl processDefinition, DbSession nhSession)
        {
            ProcessInstanceImpl processInstance = new ProcessInstanceImpl("", processDefinition);

            return processInstance;
        }

        public void ProcessTransition(TransitionImpl transition,FlowImpl flow) 
        {
            NodeImpl destination = (NodeImpl)transition.To;
            flow.Node = destination;

            if (destination is ActivityStateImpl)
            {
                ProcessActivityState((ActivityStateImpl)destination, flow);
            }
            else if (destination is ProcessStateImpl)
            {
                ProcessProcessState((ProcessStateImpl)destination, flow);
            }
            else if (destination is DecisionImpl)
            {
                ProcessDecision((DecisionImpl)destination, flow);
            }
            else if (destination is ForkImpl)
            {
                ProcessFork((ForkImpl)destination, flow);
            }
            else if (destination is JoinImpl)
            {
                ProcessJoin((JoinImpl)destination, flow);
            }
            else if (destination is EndStateImpl)
            {
                ProcessEndState((EndStateImpl)destination, flow);
            }
            else
            {
                throw new SystemException("");
            }
        }

        public void ProcessActivityState(ActivityStateImpl activityState, FlowImpl flow)
        {
            //String actorId = null;
            //String role = activityState.ActorRoleName;
            //DelegationImpl assignmentDelegation = activityState.AssignmentDelegation;

            //if (assignmentDelegation != null)
            //{
            //    // delegate the assignment of the activity-state
            //    actorId = delegationHelper.DelegateAssignment(activityState.AssignmentDelegation, executionContext);
            //    if ((Object)actorId == null)
            //    {
            //        throw new SystemException("invalid process definition : assigner of activity-state '" + activityState.Name + "' returned null instead of a valid actorId");
            //    }
            //}
            //else
            //{
            //    // get the assigned actor from the specified attribute instance
            //    if ((Object)role != null)
            //    {
            //        IActor actor = (IActor)executionContext.GetAttribute(role);
            //        if (actor == null)
            //        {
            //            throw new SystemException("invalid process definition : activity-state must be assigned to role '" + role + "' but that attribute instance is null");
            //        }
            //        actorId = actor.Id;
            //    }
            //    else
            //    {
            //        throw new SystemException("invalid process definition : activity-state '" + activityState.Name + "' does not have an assigner or a role");
            //    }
            //}

            //flow.ActorId = actorId;

            //// If necessary, store the actor in the role
            //if (((Object)role != null) && (assignmentDelegation != null))
            //{
            //    executionContext.StoreRole(actorId, activityState);
            //}

            //// the client of performActivity wants to be Informed of the people in charge of the process
            //executionContext.AssignedFlows.Add(flow);

        }

        public void ProcessProcessState(ProcessStateImpl processState, FlowImpl flow)
        {
           
        }

        public void ProcessDecision(DecisionImpl decision, FlowImpl flow)
        {
           
        }

        public void ProcessFork(ForkImpl fork, FlowImpl flow)
        {
           
        }

        public void ProcessJoin(JoinImpl join, FlowImpl flow)
        {
           
        }

        public void ProcessEndState(EndStateImpl endState, FlowImpl flow)
        {
           
        }
    }
}
