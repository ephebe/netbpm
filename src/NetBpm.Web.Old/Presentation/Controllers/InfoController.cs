using System;
using System.Threading;
using Castle.MonoRail.Framework;
using NetBpm.Web.Presentation.Helper;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	[Helper( typeof(InfoHelper) )]
	public class InfoController  : AbstractSecureController
	{
		public void ShowHome()
		{
			Context.Flash["assemblies"]= Thread.GetDomain().GetAssemblies();
			Context.Flash["facilities"]= NetBpmContainer.Instance.Kernel.GetFacilities();
			Context.Flash["baseDirectory"]=Thread.GetDomain().BaseDirectory;
			Context.Flash["dynamicDirectory"]=Thread.GetDomain().DynamicDirectory;
			Context.Flash["maxGeneration"] = (GC.MaxGeneration/1024).ToString();
			Context.Flash["totalMemory"] = (GC.GetTotalMemory(false)/1024).ToString();
		}
	}
}
