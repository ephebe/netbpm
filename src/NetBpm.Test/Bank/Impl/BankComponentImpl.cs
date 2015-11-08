using System;
using NetBpm.Util.DB;
using NHibernate.Type;
using log4net;

namespace NetBpm.Test.Bank.Impl
{
	public class BankComponentImpl
	{
		private static readonly BankComponentImpl instance = new BankComponentImpl();
		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (BankComponentImpl));

		/// <summary> gets the singleton instance.</summary>
		public static BankComponentImpl Instance
		{
			get { return instance; }
		}

		private const String queryFindBankAccount = "select b " +
			"from b in class NetBpm.Test.Bank.BankAccount " +
			"where b.BankName = ? " +
			"  and b.Customer = ?";

		public BankAccount GetBankAccount(string bank, string costumer, DbSession dbSession)
		{
			log.Debug("GetBankAccount");
			BankAccount bankAccount = null;

			Object[] values = new Object[] {bank, costumer};
			IType[] types = new IType[] {DbType.STRING, DbType.STRING};

			bankAccount = (BankAccount) dbSession.FindOne(queryFindBankAccount,values,types);
			return bankAccount;
		}

		public void CreateAccount(BankAccount bankAccount, DbSession dbSession)
		{
			log.Debug("CreateAccount");
			dbSession.Save(bankAccount);
		}

		public void DeleteAccount(string bank, string costumer, DbSession dbSession)
		{
			log.Debug("DeleteAccount");

			Object[] values = new Object[] {bank, costumer};
			IType[] types = new IType[] {DbType.STRING, DbType.STRING};

			dbSession.Delete(queryFindBankAccount,values,types);
		}

		public void Withdraw(string bank, string costumer,float amount, DbSession dbSession)
		{
			Deposid(bank, costumer, -amount, dbSession);
		}
		
		public void Deposid(string bank, string costumer,float amount, DbSession dbSession)
		{
			BankAccount bankAccount = GetBankAccount(bank, costumer, dbSession);
			bankAccount.Value=bankAccount.Value+amount;
			dbSession.Save(bankAccount);
		}
	}
}
