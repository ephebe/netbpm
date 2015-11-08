using System;
using System.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NHibernate.Type;

namespace NetBpm.Workflow.Organisation.Impl
{
	public class OrganisationRepository
	{
		private static readonly NetBpm.Workflow.Organisation.Impl.OrganisationRepository instance = new NetBpm.Workflow.Organisation.Impl.OrganisationRepository();
		private static readonly ILog log = LogManager.GetLogger(typeof (NetBpm.Workflow.Organisation.Impl.OrganisationRepository));

		/// <summary> gets the singleton instance.</summary>
		public static NetBpm.Workflow.Organisation.Impl.OrganisationRepository Instance
		{
			get { return instance; }

		}

		private OrganisationRepository()
		{
		}

		// hibernate queries //////////////////////////////////////////////////
		private const String queryFindActorById = "from a in class NetBpm.Workflow.Organisation.Impl.ActorImpl "
			+ "where a.id = ?";

		private const String queryFindAllUsers = "from u in class NetBpm.Workflow.Organisation.Impl.UserImpl";

		private const String queryFindUsersByGroupAndRole = "select u " + "from u in class NetBpm.Workflow.Organisation.Impl.UserImpl, "
			+ "     m in class NetBpm.Workflow.Organisation.Impl.MembershipImpl, "
			+ "     g in class NetBpm.Workflow.Organisation.Impl.GroupImpl "
			+ "where m.User = u "
			+ "  and m.Group = g "
			+ "  and g.id = ? "
			+ "  and m.Role = ? ";

		private const String queryFindMembershipsByUserAndGroup = "select m " + "from u in class NetBpm.Workflow.Organisation.Impl.UserImpl, "
			+ "     m in class NetBpm.Workflow.Organisation.Impl.MembershipImpl, "
			+ "     g in class NetBpm.Workflow.Organisation.Impl.GroupImpl "
			+ "where m.User = u "
			+ "  and m.Group = g "
			+ "  and u.id = ? "
			+ "  and g.id = ? ";

		private const String queryFindGroupByMembership = "select g " + "from u in class NetBpm.Workflow.Organisation.Impl.UserImpl, "
			+ "     m in class NetBpm.Workflow.Organisation.Impl.MembershipImpl, "
			+ "     g in class NetBpm.Workflow.Organisation.Impl.GroupImpl "
			+ "where m.User = u "
			+ "  and m.Group = g "
			+ "  and u.id = ? "
			+ "  and m.Type = ? ";


		// default parameter methods ////////////////////////////////////////////
		public IActor FindActorById(String actorName, DbSession dbSession)
		{
			return FindActorById(actorName, null, dbSession);
		}

		public IList FindAllUsers(DbSession dbSession)
		{
			return FindAllUsers(null, dbSession);
		}

		public IList FindUsersByGroupAndRole(String groupId, String role, DbSession dbSession)
		{
			return FindUsersByGroupAndRole(groupId, role, null, dbSession);
		}

		public IList FindMembershipsByUserAndGroup(String userId, String groupId, DbSession dbSession)
		{
			return FindMembershipsByUserAndGroup(userId, groupId, null, dbSession);
		}

		public IGroup FindGroupByMembership(String userId, String membershipType, DbSession dbSession)
		{
			return FindGroupByMembership(userId, membershipType, null, dbSession);
		}

		public IGroup CreateGroup(String groupId, ICollection userIds, DbSession dbSession)
		{
			return CreateGroup(groupId, userIds, null, dbSession);
		}

		// method implementations ////////////////////////////////////////////
		public IActor FindActorById(String actorName, Relations relations, DbSession dbSession)
		{
			IActor actor = null;
			try
			{
				actor = (IActor) dbSession.FindOne(queryFindActorById, actorName, DbType.STRING);
				if (relations != null)
					relations.Resolve(actor);
			}
			catch (Exception t)
			{
				throw new OrganisationRuntimeException("organisation-exception : coudn't find actor '" + actorName + "' by name : " + t.Message);
			}
			return actor;
		}

		public IList FindAllUsers(Relations relations, DbSession dbSession)
		{
			IList users = null;
			try
			{
				users = dbSession.Find(queryFindAllUsers);
				if (relations != null)
					relations.Resolve(users);
			}
			catch (Exception t)
			{
				throw new OrganisationRuntimeException("organisation-exception : coudn't find all users : " + t.Message);
			}
			return users;
		}

		public IList FindUsersByGroupAndRole(String groupId, String role, Relations relations, DbSession dbSession)
		{
			IList users = null;
			try
			{
				Object[] args = new Object[] {groupId, role};
				IType[] types = new IType[] {DbType.STRING, DbType.STRING};
				users = dbSession.Find(queryFindUsersByGroupAndRole, args, types);
				if (relations != null)
					relations.Resolve(users);
			}
			catch (Exception t)
			{
				throw new OrganisationRuntimeException("organisation-exception : coudn't find users by group '" + groupId + "' and role '" + role + "' : " + t.Message);
			}
			return users;
		}

		public IList FindMembershipsByUserAndGroup(String userId, String groupId, Relations relations, DbSession dbSession)
		{
			IList memberships = null;
			try
			{
				Object[] args = new Object[] {userId, groupId};
				IType[] types = new IType[] {DbType.STRING, DbType.STRING};
				log.Debug("findMembershipsByUserAndGroup(" + userId + "," + groupId + "): !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + memberships);
				memberships = dbSession.Find(queryFindMembershipsByUserAndGroup, args, types);
				log.Debug("findMembershipsByUserAndGroup(" + userId + "," + groupId + "): " + memberships);
				if (relations != null)
					relations.Resolve(memberships);
			}
			catch (Exception t)
			{
				throw new OrganisationRuntimeException("organisation-exception : coudn't find memberships by user '" + userId + "' and group '" + groupId + "' : " + t.Message);
			}
			return memberships;
		}

		public IGroup FindGroupByMembership(String userId, String membershipType, Relations relations, DbSession dbSession)
		{
			IGroup group = null;
			try
			{
				Object[] args = new Object[] {userId, membershipType};
				IType[] types = new IType[] {DbType.STRING, DbType.STRING};
				group = (IGroup) dbSession.FindOne(queryFindGroupByMembership, args, types);
				if (relations != null)
					relations.Resolve(group);
			}
			catch (Exception t)
			{
				throw new OrganisationRuntimeException("organisation-exception : coudn't find group by membership by user '" + userId + "' and membership-type '" + membershipType + "' : " + t.Message);
			}
			return group;
		}

		public IGroup CreateGroup(String groupId, ICollection userIds, Relations relations, DbSession dbSession)
		{
			//Group group = null;
			throw new NotSupportedException("in the organisation-component, the method createGroup is not yet implemented");
			// return group;
		}
	}
}