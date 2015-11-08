using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution.EComp;
using NUnit.Framework;
using System.Threading;
using NetBpm.Workflow.Organisation;

namespace MyTest
{
    [TestFixture]
    public class BaseTest 
    {
        protected internal ServiceLocator servicelocator = null;
        protected internal IProcessDefinitionService processDefinitionService = null;
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
            processDefinitionService = servicelocator.GetService(typeof(IProcessDefinitionService)) as IProcessDefinitionService;
            executionComponent = servicelocator.GetService(typeof(IExecutionApplicationService)) as IExecutionApplicationService;
        }

        public void DisposeContainer()
        {
            servicelocator.Release(processDefinitionService);
            processDefinitionService = null;
            servicelocator.Release(executionComponent);
            executionComponent = null;

            _container.Dispose();
            _container = null;
        }
    }
}
