using System;
using System.Collections;
using System.Threading;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation.EComp;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Workflow.Execution.EComp.Impl
{
	[Transactional]
	public class ExecutionApplicationService : NHSessionOpener, IExecutionApplicationService
	{
		private static readonly ExecutionComponentImpl implementation = ExecutionComponentImpl.Instance;
		private static readonly ILog log = LogManager.GetLogger(typeof (ExecutionApplicationService));

		public ExecutionApplicationService(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		public String ActorId
		{
			get { return Thread.CurrentPrincipal.Identity.Name; }
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList()
		{
			return GetTaskList(ActorId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList(Relations relations)
		{
			return GetTaskList(ActorId, relations);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList(String actorId)
		{
			return GetTaskList(actorId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList(String actorId, Relations relations)
		{
			IList taskLists = null;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				taskLists = implementation.GetTaskList(ActorId, actorId, relations, dbSession, organisationComponent);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return taskLists;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList(IList actorIds)
		{
			return GetTaskList(actorIds, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList GetTaskList(IList actorIds, Relations relations)
		{
			DbSession dbSession = null;
			IList taskLists = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				taskLists = implementation.GetTaskList(ActorId, actorIds, relations, dbSession, organisationComponent);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return taskLists;
		}


		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId)
		{
			return StartProcessInstance(processDefinitionId, null, null, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues)
		{
			return StartProcessInstance(processDefinitionId, attributeValues, null, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName)
		{
			return StartProcessInstance(processDefinitionId, attributeValues, transitionName, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations)
		{
			IProcessInstance processInstance = null;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				processInstance = implementation.StartProcessInstance(ActorId, processDefinitionId, attributeValues, transitionName, relations, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (ExecutionException e)
			{
				log.Error("Error when starting process instance :", e);
				throw e;
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when starting process instance:", e);
				throw new SystemException("uncaught exception : " + e.Message,e);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return processInstance;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IActivityForm GetStartForm(Int64 processDefinitionId)
		{
			IActivityForm activityForm = null;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				activityForm = implementation.GetStartForm(ActorId, processDefinitionId, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return activityForm;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IActivityForm GetActivityForm(Int64 flowId)
		{
			IActivityForm activityForm = null;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				activityForm = implementation.GetActivityForm(ActorId, flowId, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return activityForm;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList PerformActivity(Int64 flowId)
		{
			return PerformActivity(flowId, null, null, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList PerformActivity(Int64 flowId, IDictionary attributeValues)
		{
			return PerformActivity(flowId, attributeValues, null, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList PerformActivity(Int64 flowId, IDictionary attributeValues, String transitionName)
		{
			return PerformActivity(flowId, attributeValues, transitionName, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList PerformActivity(Int64 flowId, IDictionary attributeValues, String transitionName, Relations relations)
		{
			IList flows = null;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				flows = implementation.PerformActivity(ActorId, flowId, attributeValues, transitionName, relations, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (ExecutionException e)
			{
				log.Error("Error when performing activity :", e);
				throw e;
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when performing activity :", e);
				throw new SystemException("uncaught exception : " + e.Message, e);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return flows;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void DelegateActivity(Int64 flowId, String actorId)
		{
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				implementation.DelegateActivity(ActorId, flowId, actorId, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (ExecutionException e)
			{
				log.Error("Error when delegating activity :", e);
				throw e;
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when delegating activity :", e);
				throw new SystemException("uncaught exception : " + e.Message, e);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void CancelProcessInstance(Int64 processInstanceId)
		{
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				implementation.CancelProcessInstance(ActorId, processInstanceId, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (ExecutionException e)
			{
				log.Error("Error when canceling process instance :", e);
				dbSession.Transaction.Rollback();
				throw e;
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when canceling process instance :", e);
				throw new SystemException("uncaught exception : " + e.Message);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void CancelFlow(Int64 flowId)
		{
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				implementation.CancelFlow(ActorId, flowId, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (ExecutionException e)
			{
				log.Error("Error when canceling flow :", e);
				throw e;
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when canceling flow :", e);
				throw new SystemException("uncaught exception : " + e.Message);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void SaveActivity(Int64 flowId, IDictionary attributeValues)
		{
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				implementation.SaveActivity(ActorId, flowId, attributeValues, dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (Exception e)
			{
				log.Error("uncaught exception when performing activity :", e);
				throw new SystemException("uncaught exception : " + e.Message);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IFlow GetFlow(Int64 flowId)
		{
			return GetFlow(flowId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IFlow GetFlow(Int64 flowId, Relations relations)
		{
			IFlow flow = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			flow = implementation.GetFlow(ActorId, flowId, relations, dbSession);
			return flow;
		}

	}
}