using System;
using System.Security.Principal;
using System.Threading;
using Castle.MonoRail.Framework;
using log4net;

namespace NetBpm.Web.Presentation.Filters
{
	public class UserAddFilter : IFilter
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (AuthenticationCheckFilter));
		public UserAddFilter()
		{
		}

		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			context.CurrentUser = context.Session["user"] as IPrincipal;
			Thread.CurrentPrincipal = context.CurrentUser;
			return true;
		}
	}
}
