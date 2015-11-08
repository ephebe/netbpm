using System;
using System.Security.Principal;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Organisation
{
	public class PrincipalUserAdapter : IPrincipal, IIdentity
	{
		private IUser _user;

		public PrincipalUserAdapter(IUser user)
		{
			_user = user;
		}

		public PrincipalUserAdapter(String user)
		{
			_user=OrganisationUtil.Instance.GetUser(user);
		}

		public IUser User
		{
			get { return _user; }
		}

		#region IPrincipal

		public bool IsInRole(String role)
		{
			// TODO: portme Always false see class MembershipImpl
			return _user.Memberships.Contains(role);
		}

		public IIdentity Identity
		{
			get { return this; }
		}

		#endregion

		public string Name
		{
			get { return _user.Id; }
		}

		public string AuthenticationType
		{
			get { return "NetBpm"; }
		}

		public bool IsAuthenticated
		{
			get { return true; }
		}
	}
}
