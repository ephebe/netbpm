using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;
using NUnit.Framework;

namespace MyTest
{
    [TestFixture]
    public class HelloWorld1Test : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld1.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            processDefinitionService.DeployProcessArchive(b);
        }



        [Test]
        public void StartTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                IDictionary attributeValues = new Hashtable();
     
                IProcessDefinition booaction = processDefinitionService.GetProcessDefinition("Hello world 1");

                processInstance = executionComponent.StartProcessInstance(booaction.Id, attributeValues);
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
        public void TransitionFirstTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                var taskLists = executionComponent.GetTaskList("ae");

                foreach (IFlow task in taskLists)
                {
                    //跑完進入End State
                    executionComponent.PerformActivity(task.Id);
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

        [Test]
        public void WrongUserCantToEnd()
        {

            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ab");

            try
            {
                //既然指定了登入人員是ab，為何又讓流程執行?
                var taskLists = executionComponent.GetTaskList("ae");

                foreach (IFlow task in taskLists)
                {
                    IActor curentActor = task.GetActor();
                    Assert.AreEqual("ab", curentActor.Name);

                    executionComponent.PerformActivity(task.Id);
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
