using NetBpm.Util.Zip;
using NetBpm.Workflow.Definition;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyTest.Definition
{
    [TestFixture]
    public class ProcessDefinitionApplicationServiceTest
    {
        public void UnZipAndSave(string path) 
        {
            ParFile parFile = new ParFile(path);

            ProcessDefinitionApplicationService applicationService = new ProcessDefinitionApplicationService();
            applicationService.DeployProcessArchive(parFile);
        }
    }
}
