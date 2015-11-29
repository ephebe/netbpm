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

            //Insert into NBPM_ACTOR(id,subclass) values('ae','User')
        }
    }
}
