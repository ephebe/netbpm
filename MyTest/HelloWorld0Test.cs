using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyTest
{
    [TestFixture]
    public class HelloWorld0Test : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld0.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            processDefinitionService.DeployProcessArchive(b);

            IProcessDefinition pd = processDefinitionService.GetProcessDefinition("Hello world 0");
            Assert.IsNotNull(pd);
          
        }

        [Test]
        public void StartTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                IDictionary attributeValues = new Hashtable();

                IProcessDefinition booaction = processDefinitionService.GetProcessDefinition("Hello world 0");

                processInstance = executionComponent.StartProcessInstance(booaction.Id, attributeValues);

                //這時已經在First State
                Assert.IsNotNull(processInstance);
                //會產生基本的Root Flow
                Assert.IsNotNull(processInstance.RootFlow);

                /*
                 select *from [dbo].[NBPM_PROCESSINSTANCE]
                 select *from [dbo].[NBPM_FLOW]
                 select *from [dbo].[NBPM_LOG]
                 select *from [dbo].[NBPM_LOGDETAIL]
                 */
            }
            catch (ExecutionException e)
            {
                Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
            }
            finally
            {
                //      loginUtil.logout();
            }
        }
    }
}
