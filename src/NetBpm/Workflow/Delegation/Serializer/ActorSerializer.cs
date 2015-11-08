using System;
using NetBpm.Util.Client;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class ActorSerializer : AbstractConfigurable, ISerializer
	{
		private static readonly ServiceLocator serviceLocator;

		static ActorSerializer()
		{
			serviceLocator = ServiceLocator.Instance;
		}

		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if ((!(object_Renamed is IUser)) && (!(object_Renamed is IGroup)))
			{
				throw new ArgumentException("couldn't serialize " + object_Renamed);
			}

			if (object_Renamed != null)
			{
				IActor actor = (IActor) object_Renamed;
				serialized = actor.Id;
			}

			return serialized;
		}

		public Object Deserialize(String text)
		{
			if (text == null)
				return null;

			IActor actor = null;
			IOrganisationService organisationComponent = (IOrganisationService) serviceLocator.GetService(typeof (IOrganisationService));
			try
			{
					actor = organisationComponent.FindActorById(text);
					serviceLocator.Release(organisationComponent);
			}
			catch (Exception t)
			{
				throw new ArgumentException("couldn't deserialize " + text + " to a User : " + t.GetType().FullName + " : " + t.Message);
			}
			finally
			{
				serviceLocator.Release(organisationComponent);
			}

			return actor;
		}
	}
}