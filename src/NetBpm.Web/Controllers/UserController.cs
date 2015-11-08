using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Web.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult ShowHome()
        {
            IPrincipal userAdapter = new PrincipalUserAdapter("ae");
            HttpContext.User = userAdapter;
            Thread.CurrentPrincipal = userAdapter;

            IDefinitionSessionLocal definitionComponent = null;
            IExecutionSessionLocal executionComponent = null;
            try
            {
                definitionComponent = ServiceLocator.Instance.GetService(typeof(IDefinitionSessionLocal)) as IDefinitionSessionLocal;
                executionComponent = ServiceLocator.Instance.GetService(typeof(IExecutionSessionLocal)) as IExecutionSessionLocal;
                IList taskList = executionComponent.GetTaskList();
                IList processDefinitions = definitionComponent.GetProcessDefinitions();

                ViewData["taskList"] = taskList;
                ViewData["processDefinitions"] = processDefinitions;
                ViewData["preview"] = null;
                //Context.Flash["taskList"] = taskList;
                //Context.Flash["processDefinitions"] = processDefinitions;
                //Context.Flash["preview"] = preview;
            }
            finally
            {
                ServiceLocator.Instance.Release(executionComponent);
                ServiceLocator.Instance.Release(definitionComponent);
            }

            return View();
        }

        [HttpPost]
        public ActionResult ShowHome(String preview, Int32 processDefinitionId, Int32 flowId)
        {
            IDefinitionSessionLocal definitionComponent = null;
            IExecutionSessionLocal executionComponent = null;
            try
            {
                definitionComponent = ServiceLocator.Instance.GetService(typeof(IDefinitionSessionLocal)) as IDefinitionSessionLocal;
                executionComponent = ServiceLocator.Instance.GetService(typeof(IExecutionSessionLocal)) as IExecutionSessionLocal;
                IList taskList = executionComponent.GetTaskList();
                IList processDefinitions = definitionComponent.GetProcessDefinitions();
             
                if (preview != null)
                {
                    if (preview.Equals("process"))
                    {
                        if (processDefinitionId == 0)
                        {
                            ArrayList errors = new ArrayList();
                            errors.Add("when parameter 'preview' is equal to 'process', a valid parameter 'processDefinitionId' should be provided as well,");
                            ViewData["errormessages"] = errors;
                            //Context.Flash["errormessages"] = errors;
                        }

                        IProcessDefinition processDefinition = null;

                        // Get the processDefinition
                        processDefinition = definitionComponent.GetProcessDefinition(processDefinitionId);
                        ViewData["processDefinition"] = processDefinition;
                        //Context.Flash["processDefinition"] = processDefinition;
                    }
                    else if (preview.Equals("activity"))
                    {
                        if (flowId == 0)
                        {
                            ArrayList errors = new ArrayList();
                            errors.Add("when parameter 'preview' is equal to 'activity', a valid parameter 'flowId' should be provided as well,");
                            ViewData["errormessages"] = errors;
                            //Context.Flash["errormessages"] = errors;
                        }
                        //					IFlow flow = executionComponent.GetFlow(flowId, new Relations(new System.String[]{"processInstance.processDefinition"}));
                        IFlow flow = executionComponent.GetFlow(flowId);
                        ViewData["activity"] = flow.Node;
                       //Context.Flash["activity"] = flow.Node;
                        //AddImageCoordinates((IState)flow.Node);
                        ViewData["processDefinition"] = flow.ProcessInstance.ProcessDefinition;
                        //Context.Flash["processDefinition"] = flow.ProcessInstance.ProcessDefinition;
                    }
                }

                ViewData["taskList"] = taskList;
                ViewData["processDefinitions"] = processDefinitions;
                ViewData["preview"] = preview;
                //Context.Flash["taskList"] = taskList;
                //Context.Flash["processDefinitions"] = processDefinitions;
                //Context.Flash["preview"] = preview;
            }
            finally
            {
                ServiceLocator.Instance.Release(executionComponent);
                ServiceLocator.Instance.Release(definitionComponent);
            }

            return View();
        }
    }
}
