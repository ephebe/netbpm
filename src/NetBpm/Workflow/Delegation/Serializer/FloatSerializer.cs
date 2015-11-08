using System;
using log4net;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class FloatSerializer : AbstractConfigurable, ISerializer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FloatSerializer));

		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if (!(object_Renamed is Single))
			{
				throw new ArgumentException("FloatSerializer can't serialize " + object_Renamed);
			}

			if (object_Renamed != null)
			{
				serialized = object_Renamed.ToString();
			}

			return serialized;
		}

		public Object Deserialize(String text)
		{
			if ((Object) text == null)
				return null;
			return Single.Parse(text);
		}
	}
}