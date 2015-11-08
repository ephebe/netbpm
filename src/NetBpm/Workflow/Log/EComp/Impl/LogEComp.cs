using System;
using System.Collections;
//using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Log.Impl;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Workflow.Log.EComp.Impl
{
	[Transactional]
	public class LogEComp : NHSessionOpener, ILogSessionLocal
	{
		private static readonly LogComponentImpl implementation = LogComponentImpl.Instance;
//		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (LogEComp));

		public LogEComp(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorActorId, String actorId, Int64 processDefinitionId)
		{
			return FindProcessInstances(startedAfter, startedBefore, initiatorActorId, actorId, processDefinitionId, null);
		}


		[Transaction(TransactionMode.Requires)]
		public virtual IList FindProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorUserName, String actorUserName, Int64 processDefinitionId, Relations relations)
		{
			IList processInstances = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			processInstances = implementation.FindProcessInstances(startedAfter, startedBefore, initiatorUserName, actorUserName, processDefinitionId, relations, dbSession);
			return processInstances;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance GetProcessInstance(Int64 processInstanceId)
		{
			return GetProcessInstance(processInstanceId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance GetProcessInstance(Int64 processInstanceId, Relations relations)
		{
			ProcessInstanceImpl processInstance = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			processInstance = implementation.GetProcessInstance(processInstanceId, relations, dbSession);
			return processInstance;
		}

/*		private void Resolve(FlowImpl flow)
		{
			IEnumerator iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				FlowImpl subFlow = (FlowImpl) iter.Current;
				Resolve(subFlow);
			}
			IProcessInstance subProcessInstance = flow.GetSubProcessInstance();
			if (subProcessInstance != null)
			{
				Resolve((FlowImpl) subProcessInstance.RootFlow);
			}
		}
*/
		[Transaction(TransactionMode.Requires)]
		public virtual IFlow GetFlow(Int64 flowId)
		{
			return GetFlow(flowId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IFlow GetFlow(Int64 flowId, Relations relations)
		{
			FlowImpl flow = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			flow = implementation.GetFlow(flowId, relations, dbSession);
			return flow;
		}
	}
}