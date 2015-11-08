using System;
using System.Collections;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Execution.Impl;

namespace NetBpm.Workflow.Delegation.Impl
{
	public class AuthorizationHelper
	{
		private static readonly AuthorizationHelper instance = new AuthorizationHelper();

		/// <summary> gets the singleton instance.</summary>
		public static AuthorizationHelper Instance
		{
			get { return instance; }
		}

		private AuthorizationHelper()
		{
		}

		public void CheckRemoveProcessInstance(String authenticatedActorId, Int64 processInstanceId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromProcessInstanceId(processInstanceId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckRemoveProcessInstance(authenticatedActorId, processInstanceId);
			}
		}

		public void CheckRemoveProcessDefinition(String authenticatedActorId, Int64 processDefinitionId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromProcessDefinitionId(processDefinitionId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckRemoveProcessDefinition(authenticatedActorId, processDefinitionId);
			}
		}

		public void CheckStartProcessInstance(String authenticatedActorId, Int64 processDefinitionId, IDictionary attributeValues, String transitionName, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromProcessDefinitionId(processDefinitionId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckStartProcessInstance(authenticatedActorId, processDefinitionId, attributeValues, transitionName);
			}
		}

		public void CheckGetStartForm(String authenticatedActorId, Int64 processDefinitionId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromProcessDefinitionId(processDefinitionId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckGetStartForm(authenticatedActorId, processDefinitionId);
			}
		}

		public void CheckGetActivityForm(String authenticatedActorId, Int64 flowId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromFlowId(flowId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckGetActivityForm(authenticatedActorId, flowId);
			}
		}

		public void CheckPerformActivity(String authenticatedActorId, Int64 flowId, IDictionary attributeValues, String transitionName, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromFlowId(flowId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckPerformActivity(authenticatedActorId, flowId, attributeValues, transitionName);
			}
		}

		public void CheckDelegateActivity(String authenticatedActorId, Int64 flowId, String delegateActorId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromFlowId(flowId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckDelegateActivity(authenticatedActorId, flowId, delegateActorId);
			}
		}

		public void CheckCancelProcessInstance(String authenticatedActorId, Int64 processInstanceId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromProcessInstanceId(processInstanceId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckCancelProcessInstance(authenticatedActorId, processInstanceId);
			}
		}

		public void CheckCancelFlow(String authenticatedActorId, Int64 flowId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromFlowId(flowId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckCancelFlow(authenticatedActorId, flowId);
			}
		}

		public void CheckGetFlow(String authenticatedActorId, Int64 flowId, DbSession dbSession)
		{
			IAuthorizationHandler authorizationHandler = GetHandlerFromFlowId(flowId, dbSession);
			if (authorizationHandler != null)
			{
				authorizationHandler.CheckGetFlow(authenticatedActorId, flowId);
			}
		}

		private IAuthorizationHandler GetHandlerFromProcessDefinitionId(Int64 processDefinitionId, DbSession dbSession)
		{
			ProcessDefinitionImpl processDefinition = null;
			;
			try
			{
				processDefinition = (ProcessDefinitionImpl) dbSession.Load(typeof (ProcessDefinitionImpl), processDefinitionId);
			}
			catch (ObjectNotFoundException e)
			{
				throw new ArgumentException("couldn't check authorization : process definition with id '" + processDefinitionId + "' does not exist : " + e.Message);
			}
			return GetAuthorizationHandler(processDefinition);
		}

		private IAuthorizationHandler GetHandlerFromProcessInstanceId(Int64 processInstanceId, DbSession dbSession)
		{
			ProcessInstanceImpl processInstance = null;
			;
			try
			{
				processInstance = (ProcessInstanceImpl) dbSession.Load(typeof (ProcessInstanceImpl), processInstanceId);
			}
			catch (ObjectNotFoundException e)
			{
				throw new ArgumentException("couldn't check authorization : process instance with id '" + processInstanceId + "' does not exist : " + e.Message);
			}
			return GetAuthorizationHandler((ProcessDefinitionImpl) processInstance.ProcessDefinition);
		}

		private IAuthorizationHandler GetHandlerFromFlowId(Int64 flowId, DbSession dbSession)
		{
			FlowImpl flow = null;
			;
			try
			{
				flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			}
			catch (ObjectNotFoundException e)
			{
				throw new ArgumentException("couldn't check authorization : flow with id '" + flowId + "' does not exist : " + e.Message);
			}
			return GetAuthorizationHandler((ProcessDefinitionImpl) flow.ProcessInstance.ProcessDefinition);
		}

		private IAuthorizationHandler GetAuthorizationHandler(ProcessDefinitionImpl processDefinition)
		{
			IAuthorizationHandler authorizationHandler = null;
			DelegationImpl delegation = processDefinition.AuthorizationDelegation;
			if (delegation != null)
			{
				authorizationHandler = (IAuthorizationHandler) delegation.GetDelegate();
			}
			return authorizationHandler;
		}
	}
}