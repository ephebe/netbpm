using System;
using System.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Organisation.Impl;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Workflow.Organisation.EComp.Impl
{
	[Transactional]
	public class OrganisationService : NHSessionOpener, IOrganisationService
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (OrganisationService));
		private static readonly OrganisationRepository implementation = OrganisationRepository.Instance;

		public OrganisationService(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		// default parameter methods //////////////////////////////////////////////////
		[Transaction(TransactionMode.Requires)]
		public virtual IActor FindActorById(String actorId)
		{
			return FindActorById(actorId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IUser FindUserById(String userId)
		{
			return FindUserById(userId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup FindGroupById(String groupId)
		{
			return FindGroupById(groupId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindAllUsers()
		{
			return FindAllUsers(null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindUsersByGroupAndRole(String groupId, String role)
		{
			return FindUsersByGroupAndRole(groupId, role, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindMembershipsByUserAndGroup(String userName, String groupId)
		{
			return FindMembershipsByUserAndGroup(userName, groupId, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup FindGroupByMembership(String userName, String membershipType)
		{
			return FindGroupByMembership(userName, membershipType, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup CreateGroup(String groupId, ICollection userNames)
		{
			return CreateGroup(groupId, userNames, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IActor FindActorById(String actorId, Relations relations)
		{
			IActor actor = null;
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				actor = implementation.FindActorById(actorId, relations, dbSession);
			}
			catch (Exception t)
			{
				log.Error("error when finding actor by id " + actorId, t);
			}
			return actor;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IUser FindUserById(String userId, Relations relations)
		{
			return (IUser) FindActorById(userId, relations);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup FindGroupById(String groupId, Relations relations)
		{
			return (IGroup) FindActorById(groupId, relations);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindAllUsers(Relations relations)
		{
			IList users = null;
			DbSession dbSession = null;
			dbSession = OpenSession();
			users = implementation.FindAllUsers(relations, dbSession);
			return users;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindUsersByGroupAndRole(String groupId, String role, Relations relations)
		{
			IList users = null;
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				users = implementation.FindUsersByGroupAndRole(groupId, role, relations, dbSession);
			}
			catch (Exception t)
			{
				log.Error("error when finding users by group id " + groupId + " and role " + role, t);
			}
			return users;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IList FindMembershipsByUserAndGroup(String userName, String groupId, Relations relations)
		{
			IList memberships = null;
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				memberships = implementation.FindMembershipsByUserAndGroup(userName, groupId, relations, dbSession);
			}
			catch (Exception t)
			{
				log.Error("error when finding membership by username " + userName + " and group id " + groupId, t);
			}
			return memberships;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup FindGroupByMembership(String userName, String membershipType, Relations relations)
		{
			IGroup group = null;
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				group = implementation.FindGroupByMembership(userName, membershipType, relations, dbSession);
			}
			catch (Exception t)
			{
				log.Error("error when finding group by membership type " + membershipType, t);
			}
			return group;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual IGroup CreateGroup(String groupId, ICollection userNames, Relations relations)
		{
			throw new NotSupportedException("in the organisation-component, the method createGroup is not yet implemented");
		}

	}
}