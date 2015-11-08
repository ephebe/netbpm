using System;
using log4net;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class LongSerializer : AbstractConfigurable, ISerializer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (LongSerializer));

		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if (!(object_Renamed is Int64))
			{
				throw new ArgumentException("LongSerializer can't serialize " + object_Renamed);
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
			return Int64.Parse(text);
		}
	}
}