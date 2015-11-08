using System;
//using net.sf.hibernate;

namespace NetBpm.Workflow.Definition
{
	/// <summary> is the enum class for all runtime process execution events. </summary>
	public enum EventType
	{
		PROCESS_INSTANCE_START = 1,
		PROCESS_INSTANCE_END = 2,
		PROCESS_INSTANCE_CANCEL = 3,

		FLOW_START = 4,
		FLOW_END = 5,
		FLOW_CANCEL = 6,

		FORK = 7,
		JOIN = 8,
		TRANSITION = 9,
		BEFORE_DECISION = 10,
		AFTER_DECISION = 11,

		BEFORE_ACTIVITYSTATE_ASSIGNMENT = 12,
		AFTER_ACTIVITYSTATE_ASSIGNMENT = 13,
		BEFORE_PERFORM_OF_ACTIVITY = 14,
		PERFORM_OF_ACTIVITY = 15,
		AFTER_PERFORM_OF_ACTIVITY = 16,
		SUB_PROCESS_INSTANCE_START = 17,
		SUB_PROCESS_INSTANCE_COMPLETION = 18,

		ACTION = 19,
		DELEGATION_EXCEPTION = 20
	}

	public sealed class EventTypeHelper
	{
		public static EventType fromText(String text)
		{
			if (text.Equals("process-instance-start"))
			{
				return EventType.PROCESS_INSTANCE_START;
			}
			else if (text.Equals("process-instance-end"))
			{
				return EventType.PROCESS_INSTANCE_END;
			}
			else if (text.Equals("process-instance-cancel"))
			{
				return EventType.PROCESS_INSTANCE_CANCEL;
			}

			else if (text.Equals("flow-start"))
			{
				return EventType.FLOW_START;
			}
			else if (text.Equals("flow-end"))
			{
				return EventType.FLOW_END;
			}
			else if (text.Equals("subflow-cancel"))
			{
				return EventType.FLOW_CANCEL;
			}
			else if (text.Equals("fork"))
			{
				return EventType.FORK;
			}
			else if (text.Equals("join"))
			{
				return EventType.JOIN;
			}
			else if (text.Equals("transition"))
			{
				return EventType.TRANSITION;
			}
			else if (text.Equals("before-decision"))
			{
				return EventType.BEFORE_DECISION;
			}
			else if (text.Equals("after-decision"))
			{
				return EventType.AFTER_DECISION;
			}
			else if (text.Equals("before-activitystate-assignment"))
			{
				return EventType.BEFORE_ACTIVITYSTATE_ASSIGNMENT;
			}
			else if (text.Equals("after-activitystate-assignment"))
			{
				return EventType.AFTER_ACTIVITYSTATE_ASSIGNMENT;
			}
			else if (text.Equals("before-perform-of-activity"))
			{
				return EventType.BEFORE_PERFORM_OF_ACTIVITY;
			}
			else if (text.Equals("perform-of-activity"))
			{
				return EventType.PERFORM_OF_ACTIVITY;
			}
			else if (text.Equals("after-perform-of-activity"))
			{
				return EventType.AFTER_PERFORM_OF_ACTIVITY;
			}
			else if (text.Equals("sub-process-instance-start"))
			{
				return EventType.SUB_PROCESS_INSTANCE_START;
			}
			else if (text.Equals("sub-process-instance-completion"))
			{
				return EventType.SUB_PROCESS_INSTANCE_COMPLETION;
			}
			else if (text.Equals("action"))
			{
				return EventType.ACTION;
			}
			else if (text.Equals("delegation-exception"))
			{
				return EventType.DELEGATION_EXCEPTION;
			}
			return 0;
		}
	}
}