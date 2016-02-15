using NetBpm.Workflow.Definition;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyTest
{
    [TestFixture]
    public class HolidayNewTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/holiday.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);

            MyProcessDefinitionService service = new MyProcessDefinitionService();
            service.DeployProcessArchive(b);

            /*
             select * from [dbo].[NBPM_PROCESSBLOCK];
             select * from [dbo].[NBPM_NODE];
             select * from [dbo].[NBPM_TRANSITION];
             select * from [dbo].[NBPM_ACTION]
             select * from [dbo].[NBPM_ATTRIBUTE];
             select * from [dbo].[NBPM_DELEGATION];
             select * from [dbo].[NBPM_FIELD];
             */
        }
    }
}
