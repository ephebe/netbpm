using System;
using System.IO;
using System.Threading;
using Castle.Windsor.Configuration.Interpreters;
using NAnt.Core;
using NAnt.Core.Attributes;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Util.NAnt
{
	/// <summary>
	/// This task deploys par archives.
	/// </summary>
	[TaskName("deploypar")]
	public class DeployPar : Task
	{
		private string configfile;
		private string parfile;
		private string user;

		[TaskAttribute("configfile", Required=true)]
		public string ConfigFile
		{
			get { return configfile; }
			set { configfile = value; }
		}

		[TaskAttribute("parfile", Required=true)]
		public string ParFile
		{
			get { return parfile; }
			set { parfile = value; }
		}

		[TaskAttribute("user", Required=true)]
		public string User
		{
			get { return user; }
			set { user = value; }
		}

		protected override void ExecuteTask()
		{
			NetBpmContainer container=null;
			IProcessDefinitionService definitionComponent = null;
			try
			{
				//configure the container
				container = new NetBpmContainer(new XmlInterpreter(ConfigFile));
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IProcessDefinitionService)) as IProcessDefinitionService;
				if (definitionComponent==null)
				{
					throw new ArgumentException("Can’t create definition component. Container is not configured please check the configfile:"+ConfigFile);
				}
				Thread.CurrentPrincipal = new PrincipalUserAdapter(user);

				FileInfo parFile = new FileInfo(ParFile);
				FileStream fstream = parFile.OpenRead();
				byte[] b = new byte[parFile.Length];
				fstream.Read(b, 0, (int) parFile.Length);
				definitionComponent.DeployProcessArchive(b);				
			} finally
			{
				if (container!=null)
				{
					if (definitionComponent!=null)
					{
						container.Release(definitionComponent);
					}
					container.Dispose();
				}
			}
		}
	}
}