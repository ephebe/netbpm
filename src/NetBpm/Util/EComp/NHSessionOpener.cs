using NetBpm.Util.DB;
using NHibernate;

using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Util.EComp
{
	/// <summary>
	/// </summary>
	public class NHSessionOpener //: MarshalByRefObject
	{
		public readonly ISessionManager sessionManager;

		public NHSessionOpener(ISessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		protected DbSession OpenSession()
		{
			ISession session = sessionManager.OpenSession();
			return new DbSession(session);
		}
	}
}