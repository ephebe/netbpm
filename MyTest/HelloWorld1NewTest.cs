using NetBpm.Util.Xml;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.Impl;
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
    public class HelloWorld1NewTest : BaseTest
    {
        [Test]
        public void BuildProcessDefinitionTest()
        {
            MyProcessDefinitionBuildService service = new MyProcessDefinitionBuildService(helloWorld1());
            ProcessDefinitionImpl processDefinition = service.BuildProcessDefinition();

            Assert.AreEqual("Hello world 1", processDefinition.Name);
            Assert.AreEqual("This is the simples process.", processDefinition.Description);
            Assert.AreEqual(3, processDefinition.Nodes.Count);

            Assert.IsNotNull(processDefinition.StartState);
            Assert.AreEqual("start", processDefinition.StartState.Name);
            Assert.AreEqual(1, processDefinition.StartState.LeavingTransitions.Count);

            foreach (var node in processDefinition.Nodes)
            {
                INode no = node as INode;
                if (no != null && no.Name == "first activity state")
                {
                    Assert.AreEqual("this is the first state", no.Description);
                    Assert.AreEqual(1, no.LeavingTransitions.Count);

                    ActivityStateImpl activityState = no as ActivityStateImpl;
                    if (activityState != null)
                    {
                        Assert.IsNotNull(activityState.AssignmentDelegation);
                        Assert.AreEqual("NetBpm.Workflow.Delegation.Impl.Assignment.AssignmentExpressionResolver, NetBpm", activityState.AssignmentDelegation.ClassName);
                        Assert.AreEqual("<cfg><parameter name=\"expression\">processInitiator</parameter></cfg>", activityState.AssignmentDelegation.Configuration);
                    }
                }
            }
        }

        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld1.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);

            MyProcessDefinitionService service = new MyProcessDefinitionService();
            service.DeployProcessArchive(b);
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            MyProcessDefinitionService myProcessDefinitionService = new MyProcessDefinitionService();
            ProcessExecutionApplicationService processExecutionApplicationService = new ProcessExecutionApplicationService();

            try
            {
                IDictionary attributeValues = new Hashtable();

                processInstance = processExecutionApplicationService.StartProcessInstance(1L, attributeValues);

                //這時已經在First State
                Assert.IsNotNull(processInstance);
                //會產生基本的Root Flow
                Assert.IsNotNull(processInstance.RootFlow);
                //root flow進入了ActivityState，Id=3
                Assert.AreEqual(3, processInstance.RootFlow.Node.Id);
                //root flow的actor是ae
                Assert.AreEqual("ae", processInstance.RootFlow.GetActor().Id);
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
                    //跑完進入End State
                    flows = processExecutionApplicationService.PerformActivity(task.Id);
                }

                Assert.AreEqual(1, flows.Count);
                //跳到EndState
                Assert.AreEqual(2, ((FlowImpl)flows[0]).Node.Id);
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

        public XmlElement helloWorld1()
        {
            /*
             <?xml version="1.0"?>

            <process-definition>

              <name>Hello world 1</name>
              <description>This is the simples process.</description>

              <start-state name="start">
                <transition to="first activity state" />
              </start-state>

              <end-state name="end" />

              <activity-state name="first activity state">
                <description>this is the first state</description>
                <assignment handler="NetBpm.Workflow.Delegation.Impl.Assignment.AssignmentExpressionResolver, NetBpm">
                  <parameter name="expression">processInitiator</parameter>
                </assignment>
                <transition to="end" />
              </activity-state>

            </process-definition>
             */
            #region create Xml
            XmlElement xmlElement = new XmlElement("process-definition");
            XmlElement nameElement = new XmlElement("name");
            nameElement.Content.Add("Hello world 1");
            XmlElement descriptionElement = new XmlElement("description");
            descriptionElement.Content.Add("This is the simples process.");
            XmlElement startStateElement = new XmlElement("start-state");
            startStateElement.Attributes.Add("name", "start");
            XmlElement transitionElement = new XmlElement("transition");
            transitionElement.Attributes.Add("to", "first activity state");
            startStateElement.AddChild(transitionElement);
            XmlElement endStateElement = new XmlElement("end-state");
            endStateElement.Attributes.Add("name", "end");

            XmlElement activityStateElement = new XmlElement("activity-state");
            activityStateElement.Attributes.Add("name", "first activity state");
            XmlElement activityStateDescriptionElement = new XmlElement("description");
            activityStateDescriptionElement.Content.Add("this is the first state");
            XmlElement assignmentElement = new XmlElement("assignment");
            assignmentElement.Attributes.Add("handler", "NetBpm.Workflow.Delegation.Impl.Assignment.AssignmentExpressionResolver, NetBpm");
            XmlElement parameterElement = new XmlElement("parameter");
            parameterElement.Attributes.Add("name", "expression");
            parameterElement.Content.Add("processInitiator");
            assignmentElement.AddChild(parameterElement);
            XmlElement activityStateTransitionElement = new XmlElement("transition");
            activityStateTransitionElement.Attributes.Add("to", "end");
            activityStateElement.AddChild(activityStateDescriptionElement);
            activityStateElement.AddChild(assignmentElement);
            activityStateElement.AddChild(activityStateTransitionElement);

            xmlElement.AddChild(nameElement);
            xmlElement.AddChild(descriptionElement);
            xmlElement.AddChild(startStateElement);
            xmlElement.AddChild(endStateElement);
            xmlElement.AddChild(activityStateElement);
            #endregion
            return xmlElement;
        }
    }
}
