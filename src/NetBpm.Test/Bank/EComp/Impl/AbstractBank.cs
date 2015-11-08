using NetBpm.Test.Bank.Impl;
using NetBpm.Util.EComp;
using NetBpm.Util.DB;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Test.Bank.EComp.Impl
{
	[Transactional]
	public abstract class AbstractBank : NHSessionOpener
	{
		public abstract string GetName();
		public BankComponentImpl instance = BankComponentImpl.Instance;

		public AbstractBank(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void CreateCustomer(string name,float money)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				BankAccount account= new BankAccount();
				account.BankName=GetName();
				account.Customer=name;
				account.Value=money;
				instance.CreateAccount(account,dbSession);
				dbSession.Flush();
			}
			finally
			{
				if (dbSession!=null)
				{
					dbSession.Close();
				}
			}

		}

		[Transaction(TransactionMode.Requires)]
		public virtual BankAccount GetBankAccount(string name)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				return instance.GetBankAccount(GetName(),name,dbSession);
			}
			finally
			{
				if (dbSession!=null)
				{
					dbSession.Close();
				}
			}
		}
	}
}
