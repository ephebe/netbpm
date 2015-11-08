using System;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class DateSerializer : AbstractConfigurable, ISerializer
	{
		public String Serialize(Object object_Renamed)
		{
			String serailized = null;

			if (!(object_Renamed is DateTime))
			{
				throw new ArgumentException("DateSerializer can't serialize " + object_Renamed);
			}

			if (object_Renamed != null)
			{
				DateTime date = (DateTime) object_Renamed;
				serailized = Convert.ToString(date.Ticks);
			}

			return serailized;
		}

		public Object Deserialize(String text)
		{
			DateTime date = DateTime.MinValue;

			if (((Object) text != null) && (!"".Equals(text)))
			{
				try
				{
					long time = Int64.Parse(text);
					date = new DateTime(time);
				}
				catch (FormatException e)
				{
					throw new ArgumentException("can't deserialize " + text + " to an Date.", e);
				}
			}

			return date;
		}
	}
}