using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;
using NetBpm.Util.DB;

namespace NetBpm.Test.Bank.EComp.Impl
{
	[Transactional]
	public class BankBEComp : AbstractBank, IBankB
	{
		public BankBEComp(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		override public string GetName()
		{
			return "BankB"; 
		}

		[Transaction(TransactionMode.Requires)]
		public void DeposidMoney(string name,float money)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				instance.Deposid(GetName(),name,money,dbSession);
				// Ohhhh I forgot the rules ;-)
				if (money<0)
				{
					throw new AccountException("it is not allowed to transfer a negative amount of money!");
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
