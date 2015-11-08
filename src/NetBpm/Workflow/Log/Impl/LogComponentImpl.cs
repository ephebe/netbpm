using System;
using System.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.Impl;
using NHibernate.Type;

namespace NetBpm.Workflow.Log.Impl
{
	public class LogComponentImpl
	{
		private static readonly LogComponentImpl instance = new LogComponentImpl();
		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (LogComponentImpl));

		/// <summary> gets the singleton instance.</summary>
		public static LogComponentImpl Instance
		{
			get { return instance; }
		}

		private LogComponentImpl()
		{
		}


		private const String queryFindAllProcessInstances = "select distinct pi " +
			"from pi in class NetBpm.Workflow.Execution.Impl.ProcessInstanceImpl," +
			"     f in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
			"where f.ProcessInstance = pi ";

		public IList FindProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorActorId, String actorId, Int64 processDefinitionId, Relations relations, DbSession dbSession)
		{
			IList processInstances = null;
			String query = queryFindAllProcessInstances;
			ArrayList parameters = new ArrayList();
			ArrayList types = new ArrayList();

			if (startedAfter != DateTime.MinValue)
			{
				query += "and pi.StartNullable > ? ";
				parameters.Add(startedAfter);
				types.Add(DbType.DATE);
			}

			if (startedBefore != DateTime.MinValue)
			{
				query += "and pi.StartNullable < ? ";
				parameters.Add(startedBefore);
				types.Add(DbType.DATE);
			}

			if (initiatorActorId != null && initiatorActorId != "")
			{
				query += "and pi.InitiatorActorId = ? ";
				parameters.Add(initiatorActorId);
				types.Add(DbType.STRING);
			}

			if (actorId != null && actorId != "")
			{
				query += "and f.ActorId = ? ";
				parameters.Add(actorId);
				types.Add(DbType.STRING);
			}

			if (processDefinitionId != 0)
			{
				query += "and pi.ProcessDefinition.Id = ? ";
				parameters.Add(processDefinitionId);
				types.Add(DbType.LONG);
			}

			query += "order by pi.StartNullable desc";

			log.Debug("query for searching process instances : '" + query + "'");

			Object[] parameterArray = parameters.ToArray();
			IType[] typeArray = (IType[]) types.ToArray(typeof (IType));

			processInstances = dbSession.Find(query, parameterArray, typeArray);

			if (relations != null)
			{
				relations.Resolve(processInstances);
			}

			log.Debug("process instances : '" + processInstances + "'");

			return processInstances;
		}

		public ProcessInstanceImpl GetProcessInstance(Int64 processInstanceId, Relations relations, DbSession dbSession)
		{
			ProcessInstanceImpl processInstance = null;
			log.Debug("searching for process instances...");
			processInstance = (ProcessInstanceImpl) dbSession.Load(typeof (ProcessInstanceImpl), processInstanceId);
			Resolve((FlowImpl) processInstance.RootFlow, relations, dbSession);
			return processInstance;
		}

		private void Resolve(FlowImpl flow, Relations relations, DbSession dbSession)
		{
			// resolve the flow 
			if (relations != null)
			{
				log.Debug("resolving relations : '" + relations + "' on flow '" + flow + "'");
				relations.Resolve(flow);
			}

			// resolve the flow-details 
			IEnumerator iter = flow.Logs.GetEnumerator();
			while (iter.MoveNext())
			{
				LogImpl logImpl = (LogImpl) iter.Current;
				IEnumerator detailsIter = logImpl.Details.GetEnumerator();
				while (detailsIter.MoveNext())
				{
					LogDetailImpl LogDetailImpl = (LogDetailImpl) detailsIter.Current;
					LogDetailImpl.Resolve(dbSession);
				}
			}

			// resolve the attribute values
			iter = flow.AttributeInstances.GetEnumerator();
			while (iter.MoveNext())
			{
				AttributeInstanceImpl attributeInstance = (AttributeInstanceImpl) iter.Current;
				log.Debug("resolving attribute instance : " + attributeInstance.GetValue());
			}

			// resolve the child-flows 
			iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				FlowImpl subFlow = (FlowImpl) iter.Current;
				Resolve(subFlow, relations, dbSession);
			}

			// resolve the sub-process-flows 
			IProcessInstance subProcessInstance = flow.GetSubProcessInstance();
			if (subProcessInstance != null)
			{
				Resolve((FlowImpl) subProcessInstance.RootFlow, relations, dbSession);
			}
		}

		public FlowImpl GetFlow(Int64 flowId, Relations relations, DbSession dbSession)
		{
			FlowImpl flow = null;
			log.Debug("searching for flow '" + flowId + "'...");
			flow = (FlowImpl) dbSession.Load(typeof (FlowImpl), flowId);
			Resolve(flow, relations, dbSession);
			return flow;
		}
	}
}