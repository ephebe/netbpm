using System;
using System.Collections;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Example.Delegate
{
	public class HolidayProcessInvoker : IProcessInvocationHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (HolidayProcessInvoker));

		public String GetStartTransitionName(IProcessInvocationContext processInvokerContext)
		{
			return null;
		}

		public IDictionary GetStartAttributeValues(IProcessInvocationContext processInvokerContext)
		{
			IDictionary attributes = new Hashtable();
			
			attributes["start date"] = System.DateTime.Now;
			attributes["end date"] = new System.DateTime((System.DateTime.Now.Ticks - 621355968000000000) / 10000 + 938475344);
			attributes["comment"] = "Holiday for a new born baby !";
			
			log.Debug("attributes for the HolidayProcessInvoker: " + attributes);
			
			return attributes;
		}

		public IDictionary CollectResults(IProcessInvocationContext processInvocationContext)
		{
			IActionContext subProcessContext = processInvocationContext.GetInvokedProcessContext();

			IDictionary pregnancyAttributes = new Hashtable();
			log.Debug("collecting results ! ");
			pregnancyAttributes["holiday approval"] = subProcessContext.GetAttribute("evaluation result");

			return pregnancyAttributes;
		}

		public String GetCompletionTransitionName(IProcessInvocationContext processInvokerContext)
		{
			return null;
		}
	}
}
