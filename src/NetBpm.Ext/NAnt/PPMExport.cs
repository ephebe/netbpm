using System.Threading;
using Castle.Windsor.Configuration.Interpreters;
using NAnt.Core;
using NAnt.Core.Attributes;
using NetBpm.Ext.NAnt.EComp;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Ext.NAnt
{
	/// <summary>
	/// This task export all processdata for PPM.
	/// </summary>
	[TaskName("ppmexport")]
	public class PPMExport : Task
	{
		private string configfile;
		private string user;
		private string exportPath;

		[TaskAttribute("configfile", Required=true)]
		public string ConfigFile
		{
			get { return configfile; }
			set { configfile = value; }
		}

		[TaskAttribute("user", Required=true)]
		public string User
		{
			get { return user; }
			set { user = value; }
		}

		[TaskAttribute("exportpath", Required=true)]
		public string ExportPath
		{
			get { return exportPath; }
			set { exportPath = value; }
		}

		protected override void ExecuteTask()
		{
			NetBpmContainer container=null;
			try
			{
				//configure the container
				container = new NetBpmContainer(new XmlInterpreter(ConfigFile));
				Thread.CurrentPrincipal = new PrincipalUserAdapter(user);
				IPPMSessionLocal ppmComponent=(IPPMSessionLocal)container["PPMSession"];
				ppmComponent.ExportPPMFile(ExportPath);
			} 
			finally
			{
				if (container!=null)
				{
					container.Dispose();
				}
			}
		}

	}
}
