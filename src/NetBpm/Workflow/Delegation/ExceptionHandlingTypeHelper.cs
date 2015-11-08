using System;

namespace NetBpm.Workflow.Delegation
{
	public enum ExceptionHandlingType
	{
		ROLLBACK = 1,
		LOG = 2,
		IGNORE = 3,
	}

	public class ExceptionHandlingTypeHelper
	{
		public static ExceptionHandlingType FromText(String text)
		{
			if (text.Equals("rollback"))
			{
				return ExceptionHandlingType.ROLLBACK;
			}
			else if (text.Equals("log"))
			{
				return ExceptionHandlingType.LOG;
			}
			else if (text.Equals("ignore"))
			{
				return ExceptionHandlingType.IGNORE;
			}
			return 0;
		}
	}
}