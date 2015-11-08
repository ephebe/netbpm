using System;
using System.Collections;
using NetBpm.Util.Client;

namespace NetBpm.Workflow.Organisation.EComp
{
	public interface IOrganisationService
	{
		IActor FindActorById(String actorId);
		IActor FindActorById(String actorId, Relations relations);

		IUser FindUserById(String userId);
		IUser FindUserById(String userId, Relations relations);

		IGroup FindGroupById(String groupId);
		IGroup FindGroupById(String groupId, Relations relations);

		IList FindAllUsers();
		IList FindAllUsers(Relations relations);

		IList FindUsersByGroupAndRole(String groupId, String role);
		IList FindUsersByGroupAndRole(String groupId, String role, Relations relations);

		IList FindMembershipsByUserAndGroup(String userId, String groupId);
		IList FindMembershipsByUserAndGroup(String userId, String groupId, Relations relations);

		IGroup FindGroupByMembership(String userId, String membershipType);
		IGroup FindGroupByMembership(String userId, String membershipType, Relations relations);

		/// <summary> allows to create groups on the fly to assign an activity to
		/// the group of users that are selected for a specific activity.
		/// (feature under construction)
		/// </summary>
		IGroup CreateGroup(String groupId, ICollection userIds);

		IGroup CreateGroup(String groupId, ICollection userIds, Relations relations);
	}
}