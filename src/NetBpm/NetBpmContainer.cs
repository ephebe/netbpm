using System;
using Castle.Core.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Facilities.Logging;
using log4net;

namespace NetBpm
{
	public class NetBpmContainer : WindsorContainer
	{
		private static readonly ILog log = LogManager.GetLogger( typeof(NetBpmContainer) );
		private static NetBpmContainer instance;

		public static NetBpmContainer Instance
		{
			get {
				if (instance == null) throw new SystemException("Container not initialized");
				return instance; 
			}

		}

		public NetBpmContainer() : this(new XmlInterpreter("app_config.xml"))
		{
		}

		public NetBpmContainer(XmlInterpreter interpreter) : base(interpreter)
		{
			Init();
			if (instance != null)
			{
				log.Warn("another NebBpm container is created");
			}
			instance = this;
		}

		public override void Dispose()
		{
			base.Dispose();	
			instance=null;
		}

		private void Init(){
			RegisterFacilities();
		}

		private void RegisterFacilities()
		{
//			AddFacility("nhibernate", new NHibernateFacility());
//			AddFacility("transactions", new TransactionFacility());

			//init logging
            //MutableConfiguration confignode = new MutableConfiguration("facility");

            //confignode.Attributes.Add("loggingApi", LoggerImplementation.Log4net.ToString());
            //confignode.Attributes.Add("customLoggerFactory", String.Empty);

            //this.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", confignode);

            //AddFacility("logging", new LoggingFacility());
		}
	}
}
