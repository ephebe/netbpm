using System;
using System.Web;
using Castle.Windsor;

namespace NetBpm.Web
{
	public class NetBpmHttpApplication : HttpApplication, IContainerAccessor
	{
//		private static readonly String DefaultSessionFactory = "nhibernate.sessfactory.default";

		private static WindsorContainer container;

		public NetBpmHttpApplication()
		{
			this.BeginRequest += new EventHandler(OnBeginRequest);
			this.EndRequest += new EventHandler(OnEndRequest);
		}

		public void OnBeginRequest(object sender, EventArgs e)
		{
/*			ISessionFactory sessFac = (ISessionFactory) container[typeof (ISessionFactory)];

			ISession session = sessFac.OpenSession();

			SessionManager.Push(session, DefaultSessionFactory);

			HttpContext.Current.Items.Add("nh.session", session);
*/
		}

		public void OnEndRequest(object sender, EventArgs e)
		{
/*			ISession session = (ISession) HttpContext.Current.Items["nh.session"];

			if (session == null) return;

			try
			{
				SessionManager.Pop(DefaultSessionFactory);

				session.Dispose();
			}
			catch (Exception ex)
			{
				HttpContext.Current.Trace.Warn("Error", "EndRequest: " + ex.Message, ex);
			}
*/
		}

		public void Application_OnStart()
		{
			container = new NetBpmWebContainer();
		}

		public void Application_OnEnd()
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}