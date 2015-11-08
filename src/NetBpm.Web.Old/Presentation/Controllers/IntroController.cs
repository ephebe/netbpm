using System;
using System.Security.Principal;
using log4net;
using Castle.MonoRail.Framework;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	public class IntroController : AbstractUserAddController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (IntroController));
		public IntroController()
		{
		}

		[SkipFilter]
		public void Index()
		{
			log.Debug("index");
		}

		[SkipFilter]
		public void PerformLogin(String username,String password)
		{
			log.Debug("PerformLogin username:"+username+" password: **********");
			if (username!=null && password!=null && username.Equals(password))
			{
				InitSession(username);

				Redirect("user","showHome");
			} else {
				RenderView("intro","index");
				AddMessage("User not found or incorrect password.");
			}
		}

		[SkipFilter]
		public void Logoff()
		{
			log.Debug("Logoff");
			Context.CurrentUser=null;
			Context.Session.Clear();
			Redirect("intro","index");
		}
	

		private void InitSession(string username) 
		{
			IPrincipal userAdapter=new PrincipalUserAdapter(username);
			Context.Session["user"] = userAdapter;
			Context.CurrentUser=userAdapter;
		}
	}
}