using System;
using System.Security.Principal;
using System.Threading;
using Castle.MonoRail.Framework;
using log4net;

namespace NetBpm.Web.Presentation.Filters
{
	public class AuthenticationCheckFilter : IFilter
	{
		private IFilter _userAddFilter=new UserAddFilter();
		private static readonly ILog log = LogManager.GetLogger(typeof (AuthenticationCheckFilter));
		public AuthenticationCheckFilter()
		{
		}

		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			_userAddFilter.Perform(exec,context,controller);
			if (Thread.CurrentPrincipal!=null && Thread.CurrentPrincipal.Identity.IsAuthenticated){
				log.Debug("authenticated");
				return true;
			}
			log.Debug("not authenticated");
			controller.Redirect("intro","index");
			return false;
		}
	}
}
