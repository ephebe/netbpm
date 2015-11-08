using System;
using log4net;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class IntegerSerializer : AbstractConfigurable, ISerializer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (IntegerSerializer));

		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if (!(object_Renamed is Int32))
			{
				throw new ArgumentException("IntegerSerializer can't serialize " + object_Renamed);
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

			return Int32.Parse(text);
		}
	}
}