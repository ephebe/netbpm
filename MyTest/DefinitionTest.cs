using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution.EComp;
using NUnit.Framework;

namespace MyTest
{
    [TestFixture]
    public class DefinitionTest
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
        public void DeployProcessTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/helloworld4.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            definitionComponent.DeployProcessArchive(b);
        }

        
    }
}
