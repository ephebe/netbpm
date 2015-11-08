using System;

namespace NetBpm.Workflow.Definition
{
	public enum FieldAccess
	{
		NOT_ACCESSIBLE = 1,
		READ_ONLY = 2,
		WRITE_ONLY = 3,
		WRITE_ONLY_REQUIRED = 4,
		READ_WRITE = 5,
		READ_WRITE_REQUIRED = 6
	}

	public class FieldAccessHelper
	{
		public FieldAccessHelper()
		{
		}

		public static FieldAccess fromText(String text)
		{
			if (text.Equals("not-accessible"))
			{
				return FieldAccess.NOT_ACCESSIBLE;
			}
			else if (text.Equals("read-only"))
			{
				return FieldAccess.READ_ONLY;
			}
			else if (text.Equals("write-only"))
			{
				return FieldAccess.WRITE_ONLY;
			}
			else if (text.Equals("write-only-required"))
			{
				return FieldAccess.WRITE_ONLY_REQUIRED;
			}
			else if (text.Equals("read-write"))
			{
				return FieldAccess.READ_WRITE;
			}
			else if (text.Equals("read-write-required"))
			{
				return FieldAccess.READ_WRITE_REQUIRED;
			}
			return 0;
		}

		public static bool IsAccessible(FieldAccess fieldAccess)
		{
			return (!(fieldAccess == FieldAccess.NOT_ACCESSIBLE));
		}

		public static bool IsReadable(FieldAccess fieldAccess)
		{
			return ((fieldAccess == FieldAccess.READ_ONLY) || (fieldAccess == FieldAccess.READ_WRITE) || (fieldAccess == FieldAccess.READ_WRITE_REQUIRED));
		}

		public static bool IsWritable(FieldAccess fieldAccess)
		{
			return ((fieldAccess == FieldAccess.WRITE_ONLY) || (fieldAccess == FieldAccess.READ_WRITE) || (fieldAccess == FieldAccess.WRITE_ONLY_REQUIRED) || (fieldAccess == FieldAccess.READ_WRITE_REQUIRED));
		}

		public static bool IsRequired(FieldAccess fieldAccess)
		{
			return ((fieldAccess == FieldAccess.WRITE_ONLY_REQUIRED) || (fieldAccess == FieldAccess.READ_WRITE_REQUIRED));
		}
	}
}