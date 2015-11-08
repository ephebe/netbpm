using System;
using System.Collections;
using System.Threading;
using NetBpm.Util.Client;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Test.Workflow.Example
{
	public class TestUtil
	{
		public IList PerformActivity(String actorId, Int64 flowId, int levelsUp, IDictionary attributeValues, IExecutionApplicationService executionComponent)
		{
			IList assignedFlows = null;
			LoginUser(actorId);
			IFlow flowInList = GetFlow(levelsUp, flowId, executionComponent);
			assignedFlows = executionComponent.PerformActivity(flowInList.Id, attributeValues);
			return assignedFlows;
		}

		/// <summary> finds a flow upon which the current authenticated user has to act.
		/// It searches the flow in the current authenticated user's tasklist for which the levelsUp-parent has rootFlowId.
		/// @throws FinderException if the flow could not be found 
		/// </summary>
		public IFlow GetFlow(int levelsUp, Int64 rootFlowId, IExecutionApplicationService executionComponent)
		{
			IFlow theOne = null;

			IList flows = executionComponent.GetTaskList(new Relations(new String[] {"processInstance.processDefinition", "node", "parent"}));
			IEnumerator iter = flows.GetEnumerator();
			while (iter.MoveNext())
			{
				IFlow flow = (IFlow) iter.Current;
				IFlow rootFlow = flow;

				for (int i = 0; i < levelsUp; i++)
				{
					rootFlow = rootFlow.Parent;
				}

				if (rootFlow != null)
				{
					if (rootFlow.Id == rootFlowId)
					{
						theOne = flow;
					}
				}
			}

			if (theOne == null)
			{
				throw new SystemException("No flow in the tasklist could be found that has flow " +
					rootFlowId + " as " + levelsUp + "-levels-up-parent : " + flows);
			}

			return theOne;
		}

		public void DelegateFlow(Int64 flowId, int levelsUp, String actorId, String delegateActorId, IExecutionApplicationService executionComponent)
		{
			LoginUser(actorId);
			IFlow flowInList = GetFlow(levelsUp, flowId, executionComponent);

			// delegate the activity
			executionComponent.DelegateActivity(flowInList.Id, delegateActorId);
		}

		public void CancelFlow(String actorId, Int64 flowId, int levelsUp, IExecutionApplicationService executionComponent)
		{
			LoginUser(actorId);
			IFlow flowInList = GetFlow(levelsUp, flowId, executionComponent);
			executionComponent.CancelFlow(flowInList.Id);
		}

		public void CancelInstance(String actorId, Int64 processInstanceId, IExecutionApplicationService executionComponent)
		{
			LoginUser(actorId);
			// perform the cancel instance operaction
			executionComponent.CancelProcessInstance(processInstanceId);
		}

		public void LoginUser(String actorId)
		{
			Thread.CurrentPrincipal = new PrincipalUserAdapter(actorId);
		}

	}
}