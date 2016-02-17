using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Attr;
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
    public class HolidayNewTest : BaseTest
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
            select * from [dbo].[NBPM_PROCESSBLOCK]; --2
            select * from [dbo].[NBPM_NODE];  --9
            select * from [dbo].[NBPM_TRANSITION]; --10
            select * from [dbo].[NBPM_ACTION] --0
            select * from [dbo].[NBPM_ATTRIBUTE]; --7
            select * from [dbo].[NBPM_DELEGATION]; --11
            select * from [dbo].[NBPM_FIELD]; --0
            */
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("af");

            MyProcessDefinitionService myProcessDefinitionService = new MyProcessDefinitionService();
            ProcessExecutionService processExecutionService = new ProcessExecutionService();

            try
            {
                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("start date", new DateTime(2016, 3, 1));
                attributeValues.Add("end date", new DateTime(2016, 3, 2));

                processInstance = processExecutionService.StartProcessInstance(1L, attributeValues);

                Assert.IsNotNull(processInstance);
                Assert.IsNotNull(processInstance.RootFlow);

                /*
                 select *from [dbo].[NBPM_PROCESSINSTANCE] --1
                 select *from [dbo].[NBPM_FLOW] --1
                 select *from [dbo].[NBPM_LOG] --3
                 select *from [dbo].[NBPM_LOGDETAIL] --6
                 select * from [dbo].[NBPM_ATTRIBUTEINSTANCE] --7
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

            ProcessExecutionService processExecutionService = new ProcessExecutionService();

            try
            {
                //af申請的，ae是af的主管，登入者要換成ae
                var taskLists = processExecutionService.GetTaskList("ae");

                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("evaluation result", Evaluation.APPROVE);

                foreach (IFlow task in taskLists)
                {
                    //出現一個無法處理的錯誤
                    processExecutionService.PerformActivity(task.Id, attributeValues);
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
