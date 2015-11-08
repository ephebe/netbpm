using System;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Definition;
using NetBpm.Util.Client;

namespace NetBpm.Example.Delegate
{
	public class OvertakingAction : IActionHandler
	{
		private static readonly ServiceLocator serviceLocator = ServiceLocator.Instance;
		
		public void Run(IActionContext actionContext)
		{
			IFlow flow = actionContext.GetFlow();
			INode currentNode = flow.Node;
			
			if ("do bloody thing".Equals(currentNode.Name))
			//'do bloody thing' hasn't been performed
			{
				IExecutionApplicationService executionComponent = (IExecutionApplicationService) serviceLocator.GetService(typeof(IExecutionApplicationService));
				try
				{
					//ae is a robot. Human (in) is incapable. Let robot replace him.
					executionComponent.DelegateActivity(flow.Id, "ae");
					
					//call external component (robot web service) to act on the flow
					System.Console.Out.WriteLine("calling robot web service to act on the flow... [ok]");
					
					//Now, revenge time: director fires originally assigned executor
					System.Console.Out.WriteLine("Isaac, you're fired!");
				}
				catch (ExecutionException e)
				{
					throw new System.SystemException("failed executing task: " + typeof(OvertakingAction) + " " + e.Message);
				}
				finally
				{
					serviceLocator.Release(executionComponent);
				}
			}
		}
	}
}