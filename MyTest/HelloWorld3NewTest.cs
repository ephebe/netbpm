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
    public class HelloWorld3NewTest : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld3.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);

            MyProcessDefinitionService service = new MyProcessDefinitionService();
            //事實上，Action根本沒存進去
            service.DeployProcessArchive(b);
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            ProcessExecutionApplicationService processExecutionApplicationService = new ProcessExecutionApplicationService();

            try
            {
                IDictionary attributeValues = new Hashtable();

                processInstance = processExecutionApplicationService.StartProcessInstance(1L, attributeValues);
                //這時已經在First State
                Assert.IsNotNull(processInstance);
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

            ProcessExecutionApplicationService processExecutionApplicationService = new ProcessExecutionApplicationService();

            try
            {
                var taskLists = processExecutionApplicationService.GetTaskList("ae");
                IList flows = null;
                foreach (IFlow task in taskLists)
                {
                    //跑完進入End State,因為
                    flows = processExecutionApplicationService.PerformActivity(task.Id);
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
