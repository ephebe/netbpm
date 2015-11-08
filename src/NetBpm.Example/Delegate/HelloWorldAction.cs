using System;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Example.Delegate
{
	
	public class HelloWorldAction : IActionHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (HelloWorldAction));
		
		public void Run(IActionContext actionContext)
		{
			// Lets show that we have been here :-) 
			log.Debug(" ");
			log.Debug("H   H EEEEE L     L      OOO      W   W  OOO  RRRR  L     DDDD ");
			log.Debug("H   H E     L     L     O   O     W   W O   O R   R L     D   D");
			log.Debug("HHHHH EEE   L     L     O   O     W   W O   O R   R L     D   D");
			log.Debug("H   H E     L     L     O   O     W W W O   O RRRR  L     D   D");
			log.Debug("H   H E     L     L     O   O     W W W O   O R  R  L     D   D");
			log.Debug("H   H EEEEE LLLLL LLLLL  OOO       W W   OOO  R   R LLLLL DDDD ");
			log.Debug(" ");
			
			// The next log message will be stored in the netbpm-logs of the flow. 
			actionContext.AddLog("HELLO WORLD");
		}
	}
}