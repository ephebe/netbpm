namespace NetBpm.Test.Bank.EComp
{
	public interface IBankA
	{
		void TestTransactionCreation();
		void TransferMoney(string name,float money,IBankB bank);
		void CreateCustomer(string costumer,float amount);
		BankAccount GetBankAccount(string name);
	}
}
