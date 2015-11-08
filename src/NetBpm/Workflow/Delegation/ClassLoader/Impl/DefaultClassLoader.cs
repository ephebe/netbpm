using System;
using log4net;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Delegation.ClassLoader.Impl
{
	public class DefaultClassLoader : IClassLoader
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DefaultClassLoader));

		public Object CreateObject(DelegationImpl delegationImpl)
		{
			Object delegateClass = null;
			try
			{
				log.Debug("creating delegate '" + delegationImpl.ClassName + "'");
				Type delegationType = Type.GetType(delegationImpl.ClassName);

				delegateClass = Activator.CreateInstance(delegationType, false);

			}
			catch (Exception t)
			{
				log.Error("can't instantiate delegate '" + delegationImpl.ClassName + "' : ", t);
				throw new SystemException("can't instantiate delegate '" + delegationImpl.ClassName + "' : " + t.Message);
			}
			return delegateClass;
		}
	}
}
