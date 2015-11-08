using System;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class TextSerializer : AbstractConfigurable, ISerializer
	{
		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if (object_Renamed != null)
			{
				serialized = object_Renamed.ToString();
			}

			return serialized;
		}

		public Object Deserialize(String text)
		{
			return text;
		}
	}
}