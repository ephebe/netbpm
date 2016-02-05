using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NetBpm.Workflow.Definition.Impl;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace MyTest
{
    [TestFixture]
    public class DatabaseTest
    {
        [Test]
        public void ResetSchema()
        {
            var cfg = ConfigurationFactory.CreateSQLServer2005("NetBPM",
                                                              new string[] { "NetBpm" });

            new SchemaExport(cfg).Execute(true, true, false);

            /*
             Insert into NBPM_ACTOR(id,subclass) values('myTeam','Group')
             Insert into NBPM_ACTOR(id,subclass) values('ae','User')
             Insert into NBPM_ACTOR(id,subclass) values('af','User')
             Insert into NBPM_MEMBERSHIP(id,role,type_,user_,group_) values(1,null,'hierarchy','af','myTeam')
             Insert into NBPM_MEMBERSHIP(id,role,type_,user_,group_) values(2,'boss',null,'ae','myTeam')
             */
        }
    }
}
