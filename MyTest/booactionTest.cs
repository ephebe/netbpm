using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyTest
{
    [TestFixture]
    public class booactionTest : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/booaction.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            processDefinitionService.DeployProcessArchive(b);
        }
    }
}
