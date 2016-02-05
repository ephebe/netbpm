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
    public class attributeTest : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/attributetest.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            processDefinitionService.DeployProcessArchive(b);
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("field not accessible", "");
                attributeValues.Add("field read only", "");
                attributeValues.Add("field write only", "");
                attributeValues.Add("field write only required", "a");
                attributeValues.Add("field read write", "");
                attributeValues.Add("field read write required", "b");

                IProcessDefinition booaction = processDefinitionService.GetProcessDefinition("attribute test");

                processInstance = executionComponent.StartProcessInstance(booaction.Id, attributeValues);

                Assert.IsNotNull(processInstance);
                Assert.IsNotNull(processInstance.RootFlow);

                /*
                 select *from [dbo].[NBPM_PROCESSINSTANCE]
                 select *from [dbo].[NBPM_FLOW]
                 select *from [dbo].[NBPM_LOG]
                 select *from [dbo].[NBPM_LOGDETAIL]
                 select * from [dbo].[NBPM_ATTRIBUTE];
                 select * from [dbo].[NBPM_ATTRIBUTEINSTANCE]
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

        [Test]
        public void ProcessActivityTest()
        {
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                var taskLists = executionComponent.GetTaskList("ae");

                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("field not accessible", "");
                attributeValues.Add("field read only", "");
                attributeValues.Add("field write only", "");
                attributeValues.Add("field write only required", "a");
                attributeValues.Add("field read write", "");
                attributeValues.Add("field read write required", "b");

                foreach (IFlow task in taskLists)
                {
                    executionComponent.PerformActivity(task.Id, attributeValues);
                }
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
