using System;

namespace NetBpm.Test.Bank.EComp
{
	public interface IBankB
	{
		void DeposidMoney(string name,float money);
		void CreateCustomer(string costumer,float amount);
		BankAccount GetBankAccount(string name);

	}
}
