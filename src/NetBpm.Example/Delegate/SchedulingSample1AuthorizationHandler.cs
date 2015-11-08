using System;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation;
using NetBpm.Util.Client;
using log4net;
using Iesi.Collections;
using System.Collections;

namespace NetBpm.Example.Delegate
{
	
	public class SchedulingSample1AuthorizationHandler : IAuthorizationHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (SchedulingSample1AuthorizationHandler));
		private static readonly ServiceLocator serviceLocator = ServiceLocator.Instance;
		
		public void  CheckRemoveProcessInstance(String authenticatedActorId, Int64 processInstanceId)
		{
		}
		
		public void  CheckRemoveProcessDefinition(String authenticatedActorId, Int64 processDefinitionId)
		{
		}
		
		public void  CheckStartProcessInstance(String authenticatedActorId, Int64 processDefinitionId, IDictionary attributeValues, String transitionName)
		{
		}
		
		public void  CheckGetStartForm(String authenticatedActorId, Int64 processDefinitionId)
		{
		}
		
		public void  CheckGetActivityForm(String authenticatedActorId, Int64 flowId)
		{
		}
		
		public void  CheckPerformActivity(String authenticatedActorId, Int64 flowId, IDictionary attributeValues, String transitionName)
		{
			
			//only actor assigned for that activity can perform an activity
			IExecutionApplicationService executionComponent = (IExecutionApplicationService) serviceLocator.GetService(typeof(IExecutionApplicationService));
			try
			{
				IFlow flow = executionComponent.GetFlow(flowId);
				if (flow.GetActor().Id.Equals(authenticatedActorId) == false)
				{
					throw new AuthorizationException("only actor assigned for that activity can perform an activity");
				}
			}
			catch (ExecutionException e)
			{
				log.Error("failed doing authorization : ", e);
				throw new System.SystemException("failed doing authorization : " + e.Message);
			}
			finally
			{
				serviceLocator.Release(executionComponent);
			}		
		}
		
		public void  CheckDelegateActivity(String authenticatedActorId, Int64 flowId, String delegateActorId)
		{
			//only director can delegate an activity
			IExecutionApplicationService executionComponent = (IExecutionApplicationService) serviceLocator.GetService(typeof(IExecutionApplicationService));
			try
			{
				IFlow flow = executionComponent.GetFlow(flowId, new Relations("attributeInstances"));
				IActor director = null;
				
				ISet attributeInstances = flow.AttributeInstances;
				for (IEnumerator iter = attributeInstances.GetEnumerator(); iter.MoveNext(); )
				{
					IAttributeInstance attributeInstance = (IAttributeInstance) iter.Current;
					if ("director".Equals(attributeInstance.Attribute.Name))
					{
						director = (IActor) attributeInstance.GetValue();
					}
				}
				
				if (director.Id.Equals(authenticatedActorId) == false)
				{
					throw new AuthorizationException("Only director is allowed to delegate activity");
				}
			}
			catch (AuthorizationException e)
			{
				throw e;
			}
			catch (System.Exception e)
			{
				log.Error("failed doing authorization : ", e);
				throw new System.SystemException("failed doing authorization : " + e.Message);
			}
			finally
			{
				serviceLocator.Release(executionComponent);
			}
		}
		
		public void  CheckCancelProcessInstance(String authenticatedActorId, Int64 processInstanceId)
		{
		}
		
		public void  CheckCancelFlow(String authenticatedActorId, Int64 flowId)
		{
		}
		
		public void  CheckGetFlow(String authenticatedActorId, Int64 flowId)
		{
		}
	}
}