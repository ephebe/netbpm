using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation.EComp;
using NUnit.Framework;

namespace MyTest
{
    public class OrgansationTest
    {
        protected internal ServiceLocator servicelocator = null;
        protected internal IOrganisationService _organisationService = null;
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
            _organisationService = servicelocator.GetService(typeof(IOrganisationService)) as IOrganisationService;

        }

        public void DisposeContainer()
        {
            servicelocator.Release(_organisationService);
            _organisationService = null;

            _container.Dispose();
            _container = null;
        }

        [Test]
        public void CreateUserTest()
        {
            try
            {

                var group =  _organisationService.CreateGroup("", new ArrayList() {"ae"});

             
                Assert.IsNotNull(group);
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
