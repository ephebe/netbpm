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
using NetBpm.Workflow.Definition.Attr;

namespace MyTest
{
    public class HelloWorld4Test : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld4.par");
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

                IProcessDefinition booaction = processDefinitionService.GetProcessDefinition("Hello world 4");

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
        public void ProcessActivityTest()
        {
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                var taskLists = executionComponent.GetTaskList("ae");

                IDictionary attributeValues = new Hashtable();
                //對應NetBpm.Workflow.Delegation.Impl.Serializer.EvaluationSerializer，否則無法存入[Attribute]
                //也對應NetBpm.Workflow.Delegation.Impl.Decision.EvaluationDecision，才會判斷是否轉到另一關卡，沒給只會一直在Decision
                attributeValues.Add("evaluation result", Evaluation.APPROVE);
                foreach (IFlow task in taskLists)
                {
                    //先驗證是否為processInitiator
                    //執行進入Decision前，有一Action
                    //檢查傳入的參數是否為approve
                    //是就跑到結束
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
