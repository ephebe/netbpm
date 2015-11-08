using System;
using NetBpm.Ext.NAnt.EComp;
using NetBpm.Test;
using NetBpm.Util.Client;
using NUnit.Framework;
using Castle.Windsor.Configuration.Interpreters;

namespace NetBpm.Ext.Test.PPM
{
	[TestFixture]
	public class PPMComponentTest
	{
		private ServiceLocator serviceLocator;
		private NetBpm.NetBpmContainer container = null;

		[SetUp]
		public void SetUp()
		{
			//configure the container
			container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"ppm_app_config.xml"));
			serviceLocator = ServiceLocator.Instance;
		}

		[TearDown]
		public void TearDown()
		{
			container.Dispose();
			container = null;
			serviceLocator = null;
		}

		[Test]
		public void TestExport()
		{
			IPPMSessionLocal ppmComponent = null;
			try
			{
				ppmComponent=(IPPMSessionLocal)container["PPMSession"];
				ppmComponent.ExportPPMFile(TestHelper.GetConfigDir()+"/..");
			}
			finally
			{
				container.Release(ppmComponent);				
			}
			
		}
	}
}
