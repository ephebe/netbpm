using System;
using System.Collections;
using log4net;
using Castle.MonoRail.Framework;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	public class UserController : AbstractSecureController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (UserController));

		public UserController()
		{
		}

		public void ShowHome(String preview,Int32 processDefinitionId,Int32 flowId)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("ShowHome preview:"+preview+" processDefinitionId:"+processDefinitionId+" flowId:"+flowId);
			}
			IDefinitionSessionLocal definitionComponent = null;
			IExecutionSessionLocal executionComponent = null;
			try
			{
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IDefinitionSessionLocal)) as IDefinitionSessionLocal;
				executionComponent = ServiceLocator.Instance.GetService(typeof (IExecutionSessionLocal)) as IExecutionSessionLocal;
				//			IList taskList = executionComponent.GetTaskList(new Relations(new System.String[]{"processInstance.processDefinition"}));
				IList taskList = executionComponent.GetTaskList();
				IList processDefinitions = definitionComponent.GetProcessDefinitions();
				// Collect data for the preview
				if (preview != null)
				{
					if (preview.Equals("process"))
					{
						if (processDefinitionId == 0)
						{
							ArrayList errors = new ArrayList();
							errors.Add("when parameter 'preview' is equal to 'process', a valid parameter 'processDefinitionId' should be provided as well,");
							Context.Flash["errormessages"] = errors;
						}
					
						IProcessDefinition processDefinition = null;
					
						// Get the processDefinition
						processDefinition = definitionComponent.GetProcessDefinition(processDefinitionId);
						Context.Flash["processDefinition"]=processDefinition;
					}
					else if (preview.Equals("activity"))
					{
						if (flowId == 0)
						{
							ArrayList errors = new ArrayList();
							errors.Add("when parameter 'preview' is equal to 'activity', a valid parameter 'flowId' should be provided as well,");
							Context.Flash["errormessages"] = errors;
						}
						//					IFlow flow = executionComponent.GetFlow(flowId, new Relations(new System.String[]{"processInstance.processDefinition"}));
						IFlow flow = executionComponent.GetFlow(flowId);
						Context.Flash["activity"] = flow.Node;
						AddImageCoordinates((IState)flow.Node);
						Context.Flash["processDefinition"]=flow.ProcessInstance.ProcessDefinition;
					}
				}
			
				Context.Flash["taskList"] = taskList;
				Context.Flash["processDefinitions"] = processDefinitions;
				Context.Flash["preview"] = preview;
			} 
			finally
			{
				ServiceLocator.Instance.Release(executionComponent);
				ServiceLocator.Instance.Release(definitionComponent);
			}

		}
	}
}