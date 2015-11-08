using System;
using NetBpm.Util.Client;
using NetBpm.Workflow.Organisation.EComp;

namespace NetBpm.Workflow.Organisation
{
	public class OrganisationUtil
	{
		private static readonly ServiceLocator serviceLocator = ServiceLocator.Instance;
		private static readonly OrganisationUtil instance = new OrganisationUtil();

		/// <summary> gets the singleton instance.</summary>
		public static OrganisationUtil Instance
		{
			get { return instance; }
		}

		private OrganisationUtil()
		{
		}

		public IActor GetActor(String actorId)
		{
			IActor actor = null;

			if ((Object) actorId != null)
			{
				IOrganisationService organisationComponent = null;
				try
				{
					organisationComponent = (IOrganisationService) serviceLocator.GetService(typeof (IOrganisationService));
					actor = organisationComponent.FindActorById(actorId);
				}
				catch (Exception t)
				{
					throw new SystemException("couldn't get actor '" + actorId + "' from the organisation-component : " + t.Message);
				}
				finally
				{
					serviceLocator.Release(organisationComponent);
				}
			}

			return actor;
		}

		public IUser GetUser(String userId)
		{
			IUser user = null;
			IOrganisationService organisationComponent = null;
			try
			{
				organisationComponent = (IOrganisationService) serviceLocator.GetService(typeof (IOrganisationService));
				user = organisationComponent.FindUserById(userId);
			}
			catch (Exception t)
			{
				throw new SystemException("couldn't get user '" + userId + "' from the organisation-component : " + t.Message);
			}
			finally
			{
				serviceLocator.Release(organisationComponent);
			}
			return user;
		}

		public IGroup GetGroup(String groupId)
		{
			IGroup group = null;
			IOrganisationService organisationComponent = null;
			try
			{
				organisationComponent = (IOrganisationService) serviceLocator.GetService(typeof (IOrganisationService));
				group = organisationComponent.FindGroupById(groupId);
			}
			catch (Exception t)
			{
				throw new SystemException("couldn't get group '" + groupId + "' from the organisation-component : " + t.Message);
			}
			finally
			{
				serviceLocator.Release(organisationComponent);
			}
			return group;
		}
	}
}