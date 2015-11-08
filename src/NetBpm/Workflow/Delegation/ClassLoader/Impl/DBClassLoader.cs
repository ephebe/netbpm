using System;
using System.Reflection;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Delegation.Impl;
using NHibernate.Type;

namespace NetBpm.Workflow.Delegation.ClassLoader.Impl
{
	[Transactional]
	public class DBClassLoader : NHSessionOpener, IClassLoader
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DefaultClassLoader));

		public DBClassLoader(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		private const String queryFindAssemblyFile = "select cf " +
			"from cf in class NetBpm.Workflow.Delegation.Impl.AssemblyFileImpl " +
			"where ( cf.ProcessDefinition.Id = ? ) " +
			"  and ( cf.AssemblyName = ? ) ";

		[Transaction(TransactionMode.Requires)]
		public Object CreateObject(DelegationImpl delegationImpl)
		{
			Object delegateClass = null;
			try
			{
				log.Debug("creating delegate '" + delegationImpl.ClassName + "'");
				if (delegationImpl.ClassName.IndexOf(",") != -1)
				{
					int index = delegationImpl.ClassName.IndexOf(",");
					string className = delegationImpl.ClassName.Substring(0, index).Trim();
					string assemblyName = delegationImpl.ClassName.Substring(index + 1).Trim();

					DbSession dbSession = null;
					AssemblyFileImpl assemblyFile = null;
					try
					{
						dbSession = OpenSession();
						Object[] args = new Object[] {delegationImpl.ProcessDefinition.Id, assemblyName};
						IType[] types = new IType[] {DbType.LONG, DbType.STRING};
						assemblyFile = (AssemblyFileImpl) dbSession.FindOne(queryFindAssemblyFile, args, types);
					} 
					catch (ObjectNotFoundException onfe){}
					finally
					{
						if (dbSession!=null)
						{
							dbSession.Close();
						}
					}

					if (assemblyFile != null)
					{
						Assembly loaded = Assembly.Load(assemblyFile.Bytes);
						Type dbDelegationType =  loaded.GetType(className);
						delegateClass = Activator.CreateInstance(dbDelegationType, false);
						return delegateClass;
					}

				}
				//load Assembly from environment
				log.Debug("load Assembly from environment");
				Type delegationType = Type.GetType(delegationImpl.ClassName);
				delegateClass = Activator.CreateInstance(delegationType, false);

				return delegateClass;
			}
			catch (Exception t)
			{
				log.Error("can't instantiate delegate '" + delegationImpl.ClassName + "' : ", t);
				throw new SystemException("can't instantiate delegate '" + delegationImpl.ClassName + "' : " + t.Message);
			}
		}
	}
}