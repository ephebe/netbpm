// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Castle.Facilities.AutoTx;

namespace NetBpm.Test.BaseService
{
	using System;

	using Castle.Facilities.NHibernateIntegration;

	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	
	using NUnit.Framework;
	
	using NetBpm.Test.BaseService.Model;
	using NetBpm.Test.Bank.EComp.Impl;
	using NetBpm.Test.Bank.EComp;

	[TestFixture]
	public class CastleTransactionsTest 
	{
        //protected IWindsorContainer container;

        //[SetUp]
        //public void Init()
        //{
        //    container = new WindsorContainer( GetContainerConfig() );

        //    // Reset tables

        //    Configuration cfg1 = (Configuration) container[ "sessionFactory1.cfg" ];
        //    SchemaExport export1 = new SchemaExport(cfg1);

        //    export1.Create(false, true);

        //    ConfigureContainer();
        //}

        //[TearDown]
        //public void Dispose()
        //{
        //    Configuration cfg1 = (Configuration) container[ "sessionFactory1.cfg" ];
        //    SchemaExport export1 = new SchemaExport(cfg1);

        //    export1.Drop(false, true);

        //    container.Dispose();

        //    container = null;
        //}

        //protected void ConfigureContainer()
        //{
        //    container.AddFacility( "transactions", new TransactionFacility() );
        //    container.AddComponent( "myfirstdao", typeof(FirstDao) );
        //    container.AddComponent( "myseconddao", typeof(SecondDao) );
        //    container.AddComponent( "BankA", typeof(BankAEComp) );
        //}

        //protected virtual String GetContainerConfig()
        //{
        //    return TestHelper.GetConfigDir()+"CastleTransactionConfig.xml";
        //}

        //[Test]
        //public void TestTransactionCreationCastle()
        //{
        //    ISessionManager sessionManager = (ISessionManager) 
        //        container[ typeof(ISessionManager) ];

        //    using(ISession session = sessionManager.OpenSession())
        //    {
        //        Assert.IsNull(session.Transaction);

        //        FirstDao service = (FirstDao) container["myfirstdao"];

        //        // This call is transactional
        //        Blog blog = service.Create();
        //        Assert.IsNotNull(blog);
        //        Assert.IsTrue(session.Transaction.WasCommitted);

        //    }
        //}

        //[Test]
        //public void TestTransactionCreationEComp()
        //{
        //    ISessionManager sessionManager = (ISessionManager) 
        //        container[ typeof(ISessionManager) ];

        //    using(ISession session = sessionManager.OpenSession())
        //    {
        //        Assert.IsNull(session.Transaction);

        //        IBankA myBankA = (IBankA) container["BankA"];
        //        myBankA.TestTransactionCreation();

        //        Assert.IsTrue(session.Transaction.WasCommitted);

        //    }
        //}
	}
}
