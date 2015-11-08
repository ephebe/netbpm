using System;
using log4net;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Delegation.ClassLoader;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Log.EComp;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler.EComp;
using NetBpm.Workflow.Scheduler.EComp.Impl;
using NetBpm;

namespace NetBpm.Util.Client
{
	public class ServiceLocator
	{
		private static readonly ServiceLocator instance = new ServiceLocator();
		private readonly SchedulerThread scheduler = new SchedulerThread();
		private static readonly ILog log = LogManager.GetLogger(typeof (ServiceLocator));
//		private static readonly NetBpmContainer container = NetBpmContainer.Instance;

		private ServiceLocator()
		{
		}

		/// <summary> get an instance of this service locator with default params</summary>
		/// <returns>ServiceLocator</returns>
		public static ServiceLocator Instance
		{
			get { return instance; }
		}

		public SchedulerThread Scheduler
		{
			get { return scheduler; }
		}
/*
		public NetBpmContainer Container
		{
			get { return container; }
		}
*/
		/// <summary> Get a netbpm component. Following are valid interfaceClass. If invalid 
		/// interfaceClass are supplied, an error will be logged.
		/// </summary>
		/// <param name="interfaceClass">interfaceClass
		/// </param>
		/// <returns> Object
		/// </returns>
		public Object GetService(Type interfaceClass)
		{
			Object serviceObject = null;

			try
			{
				if (interfaceClass == typeof (IOrganisationService))
				{
					serviceObject = NetBpmContainer.Instance["OrganisationSession"];
				}
				else if (interfaceClass == typeof (IProcessDefinitionService))
				{
					serviceObject = NetBpmContainer.Instance["DefinitionSession"];
				}
				else if (interfaceClass == typeof (IExecutionApplicationService))
				{
					serviceObject = NetBpmContainer.Instance["ExecutionSession"];
				}
				else if (interfaceClass == typeof (ISchedulerSessionLocal))
				{
					serviceObject = NetBpmContainer.Instance["SchedulerSession"];
				}
				else if (interfaceClass == typeof (ILogSessionLocal))
				{
					serviceObject = NetBpmContainer.Instance["LogSession"];
				}
				else if (interfaceClass == typeof (IClassLoader))
				{
					serviceObject = NetBpmContainer.Instance["ClassLoader"];
				}
				else
					throw new SystemException("couldn't get unknown service : " + interfaceClass.FullName);
			}
			catch (Exception t)
			{
				log.Error("couldn't get service " + interfaceClass.FullName, t);
				throw new SystemException("couldn't get service " + interfaceClass.FullName);
			}

			return serviceObject;
		}

		public void Release(object releaseObject)
		{
			NetBpmContainer.Instance.Release(releaseObject);
		}
	}
}