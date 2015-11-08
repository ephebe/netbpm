using System;

namespace NetBpm.Test.Bank 
{
	public class BankAccount
	{	
		private float accountValue;
		private string bankName;
		private string customer;
		private Int64 id;

        public virtual Int64 Id
		{
			get { return id; }
			set { this.id = value; }
		}

        public virtual float Value
		{
			get { return this.accountValue; }
			set { this.accountValue = value; }
		}

        public virtual string BankName
		{
			get { return this.bankName; }
			set { this.bankName = value; }
		}

        public virtual string Customer
		{
			get { return this.customer; }
			set { this.customer = value; }
		}
	}
}
