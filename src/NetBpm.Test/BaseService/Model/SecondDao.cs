using System;

using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using NetBpm.Util.EComp;
using NHibernate;

using NUnit.Framework;

namespace NetBpm.Test.BaseService.Model
{
	[Transactional]
	public class SecondDao : NHSessionOpener
	{
		private readonly ISessionManager sessManager;

		public SecondDao(ISessionManager sessManager) : base(sessManager)
		{
			this.sessManager = sessManager;
		}

		[Transaction]
		public virtual void TestTransactionCreation()
		{
			ISession session = sessManager.OpenSession();
			Assert.IsNotNull(session.Transaction);
			session.Close();
		}
	}
}
