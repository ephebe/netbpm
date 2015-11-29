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

            IProcessDefinition pd = processDefinitionService.GetProcessDefinition("Hello world 1");
            Assert.IsNotNull(pd);
            //1:first activity state 2:start 3:end
            Assert.AreEqual(3, pd.Nodes.Count);
            //也要能取出state
            //pd.GetStates("first activity state")

            //要能取出Trnsitions
            //transitions = pd.from("first activity state");
            //transitions = pd.to("end");

            //要能取得Delegations

            /*select * from [dbo].[NBPM_DELEGATION];
              select * from [dbo].[NBPM_NODE];
              select * from [dbo].[NBPM_PROCESSBLOCK];
              select * from [dbo].[NBPM_TRANSITION];
              select *from [dbo].[NBPM_DELEGATION]
            */
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
