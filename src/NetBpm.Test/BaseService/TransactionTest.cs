using Castle.Facilities.NHibernateIntegration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

using NetBpm.Test.Bank.EComp;
using NetBpm.Test.Bank;

namespace NetBpm.Test.BaseService
{
	/// <summary>
	/// This unittest tests the behaviour of Transactions
	/// </summary>
	[TestFixture]
	public class TransactionTest
	{
		private WindsorContainer container = null;
		private IBankA myBankA = null;
		private IBankB myBankB = null;

		//[TestFixtureSetUp]
		[SetUp]
		public void SetUp()
		{
			SetContainer();
			Configuration cfg = (Configuration) container[  "nhibernate.factory.cfg"  ];
			SchemaExport export = new SchemaExport(cfg);
			export.Create(false, true);

// It is not possible to add the transaction component here! See http://forum.castleproject.org/posts/list/455.page		
//			container.AddFacility( "transactions", new TransactionFacility() );

			myBankA = (IBankA) container["BankA"];
			myBankB = (IBankB) container["BankB"];
		}

		//[TestFixtureTearDown]
		[TearDown]
		public void TearDown()
		{
			container.Release(myBankA);
			container.Release(myBankB);
			myBankA=null;
			myBankB=null;

			Configuration cfg = (Configuration) container[  "nhibernate.factory.cfg" ];
			SchemaExport export = new SchemaExport(cfg);
			export.Drop(false, true);

			DisposeContainer();
		}

		public void SetContainer()
		{
			//configure the container
			container = new WindsorContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"bank_app_config.xml"));
		}

		public void DisposeContainer()
		{	
			container.Dispose();
			container = null;
		}


		[Test]
		public void TestTransactionCreationEComp()
		{
			ISessionManager sessionManager = (ISessionManager) 
				container[ typeof(ISessionManager) ];

			using(ISession session = sessionManager.OpenSession())
			{
				Assert.IsNull(session.Transaction);

				IBankA myBankA = (IBankA) container["BankA"];
				myBankA.TestTransactionCreation();

				Assert.IsTrue(session.Transaction.WasCommitted);

			}
		}

		[Test]
		public void TransactionNoException()
		{
			string customerName = "Mueller";

			myBankA.CreateCustomer(customerName,10);
			myBankB.CreateCustomer(customerName,0);

			// test created account Bank A
			BankAccount account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,10);

			// test created account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,0);

			//transfer money
			myBankA.TransferMoney(customerName,9,myBankB);

			// test account Bank A
			account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,1);

			// test account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,9);
		}

		/// <summary>
		/// The first bank will throw a exception 
		/// because it is not allowed to transfer 
		/// more then 10
		/// </summary>
		[Test]
		public void TransactionExceptionFirstLevel()
		{
			string customerName = "Mueller";

			myBankA.CreateCustomer(customerName,10);
			myBankB.CreateCustomer(customerName,0);

			// test created account Bank A
			BankAccount account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,10);

			// test created account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,0);

			try
			{
				//transfer money
				myBankA.TransferMoney(customerName,11,myBankB);
				Assert.Fail("it is not allowed to transfer more then 10");
			} catch (AccountException aex)
			{
				Assert.IsNotNull(aex);
			}

			// test account Bank A
			account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,10);

			// test account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,0);
		}

		/// <summary>
		/// The first bank will throw a exception 
		/// because it is not allowed to transfer 
		///  a negative amount 
		/// </summary>
		[Test]
		public void TransactionExceptionSecondLevel()
		{
			string customerName = "Mueller";

			myBankA.CreateCustomer(customerName,10);
			myBankB.CreateCustomer(customerName,0);

			// test created account Bank A
			BankAccount account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,10);

			// test created account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,0);

			try
			{
				//transfer money
				myBankA.TransferMoney(customerName,-1,myBankB);
				Assert.Fail("it is not allowed to transfer a negative amount ");
			} 
			catch (AccountException aex)
			{
				Assert.IsNotNull(aex);
			}

			// test account Bank A
			account = myBankA.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,10);

			// test account Bank B
			account = myBankB.GetBankAccount(customerName);
			Assert.IsNotNull(account);
			Assert.AreEqual(account.Value,0);
		}
	}
}
