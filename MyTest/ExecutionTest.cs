using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation;
using NUnit.Framework;

namespace MyTest
{
    [TestFixture]
    public class ExecutionTest
    {
        protected internal ServiceLocator servicelocator = null;
        protected internal IProcessDefinitionService definitionComponent = null;
        protected internal IExecutionApplicationService executionComponent = null;
        private NetBpmContainer _container = null;

        [SetUp]
        public void SetUp()
        {
            SetContainer();
        }

        [TearDown]
        public void TearDown()
        {
            DisposeContainer();
        }
        public void SetContainer()
        {
            //configure the container
            _container = new NetBpmContainer(new XmlInterpreter("WindsorConfig.xml"));
            servicelocator = ServiceLocator.Instance;
            definitionComponent = servicelocator.GetService(typeof(IProcessDefinitionService)) as IProcessDefinitionService;
            executionComponent = servicelocator.GetService(typeof(IExecutionApplicationService)) as IExecutionApplicationService;

        }

        public void DisposeContainer()
        {
            servicelocator.Release(definitionComponent);
            definitionComponent = null;
            servicelocator.Release(executionComponent);
            executionComponent = null;

            _container.Dispose();
            _container = null;
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                IDictionary attributeValues = new Hashtable();
                //attributeValues["available"] = "APPROVE";
                //attributeValues["product"] = "abcd";
                //attributeValues["requester"] = "efg";

                IProcessDefinition booaction = definitionComponent.GetProcessDefinition("Hello world 4");

                processInstance = executionComponent.StartProcessInstance(booaction.Id, attributeValues);
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
        public void RuntFlowTest()
        {
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                var ret = executionComponent.PerformActivity(1);
                Assert.IsNotNull(ret);
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
