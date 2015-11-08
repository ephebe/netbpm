using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;
using NetBpm.Util.DB;
using NUnit.Framework;

namespace NetBpm.Test.Bank.EComp.Impl
{
	[Transactional]
	public class BankAEComp : AbstractBank, IBankA
	{

		public BankAEComp(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		override public string GetName()
		{
			return "BankA"; 
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void TestTransactionCreation()
		{
			DbSession dbSession = OpenSession();
			Assert.IsNotNull(dbSession.Transaction);
			dbSession.Close();
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void TransferMoney(string name,float money,IBankB bank)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				instance.Withdraw(GetName(),name,money,dbSession);
				bank.DeposidMoney(name,money);
				// Ohhhh I forgot the rules ;-)
				if (money>10)
				{
					throw new AccountException("it is not allowed to transfer more then 10!");
				}
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
