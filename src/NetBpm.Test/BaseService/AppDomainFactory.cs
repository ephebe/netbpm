using System;
using System.IO;
using System.Security.Policy;

namespace NetBpm.Test.BaseService
{
	public class AppDomainFactory
	{
		public static AppDomain Create(String name)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;

			String baseDir = new FileInfo(currentDomain.BaseDirectory).FullName;

			String configFile =  String.Format(
				"{0}/{1}.config", 
				baseDir, name); 

			AppDomainSetup setup = new AppDomainSetup();

			setup.ApplicationName = name;
			setup.ApplicationBase = currentDomain.SetupInformation.ApplicationBase;
			setup.PrivateBinPath = currentDomain.SetupInformation.PrivateBinPath;
			setup.ConfigurationFile = configFile;

			Evidence baseEvidence = currentDomain.Evidence;
			Evidence evidence = new Evidence(baseEvidence);

			return AppDomain.CreateDomain(name, evidence, setup);
		}
	}
}
