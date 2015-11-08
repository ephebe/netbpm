using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NetBpm.Util.Client;
using NetBpm.Web.Models;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;

namespace NetBpm.Web.Controllers
{
    public class FormController : Controller
    {
        //
        // GET: /Form/
        public ActionResult ActivityForm(long flowId)
        {
			IExecutionSessionLocal executionComponent = null;
			try
			{
				executionComponent = ServiceLocator.Instance.GetService(typeof (IExecutionSessionLocal)) as IExecutionSessionLocal;
				IFlow flow = executionComponent.GetFlow(flowId);

				ViewData["activity"] = flow.Node;
				ViewData["processDefinition"] = flow.ProcessInstance.ProcessDefinition;
				ViewData["preview"] = "activity";
				//AddImageCoordinates((IState)flow.Node);

				//create Form
				IActivityForm activityForm = executionComponent.GetActivityForm(flowId);
				AddFormData(activityForm);
			}
			finally
			{
				ServiceLocator.Instance.Release(executionComponent);
			}

            return View();
        }


        [HttpPost]
        public ActionResult ActivityForm(string aa)
        {
			IDictionary userInputFields = new Hashtable();
			IActivityForm activityForm = (IActivityForm)HttpContext.Session["activityForm"];
			IList fields = activityForm.Fields;
			IEnumerator fildEnumer = fields.GetEnumerator();
			while (fildEnumer.MoveNext())
			{
				IField field = (IField)fildEnumer.Current;
				// Construct a meaningfull name that is http-compliant
				String attributeName = field.Attribute.Name;
				String parameterName = convertToHttpCompliant(attributeName);
				String parameterValue = HttpContext.Request.Params[parameterName];

				if (FieldAccessHelper.IsRequired(field.Access) && (parameterValue==null || "".Equals(parameterValue)))
				{
					//AddMessage("Field "+field.Name+" is required. Please, provide a value");
				} 
				else 
				{
					try
					{
						Object parsedParameter = null;
						IHtmlFormatter htmlFormatter = field.GetHtmlFormatter();
						if (htmlFormatter!=null)
						{
							// TODO: Test if there is the possibility to simplify the interface, see null
							parsedParameter = htmlFormatter.ParseHttpParameter(parameterValue,null);

							if ( parsedParameter != null ) 
							{
								userInputFields.Add( attributeName, parsedParameter );
							}				
						} 
						else
						{
							//log.Warn("No htmlformatter defined for field:"+field.Name);
						}
					} 
					catch (Exception ex)
					{
                        //log.Debug( "error parsing user-input-field " + field.Name + " : " + parameterValue,ex);
                        //AddMessage("error parsing user-input-field " + field.Name + " with value: " + parameterValue);
					}
				}
			}

			if (false ) 
			{
                //log.Debug( "submitted activity-form has messages, redirecting to activityFormPage..." );
				HttpContext.Session.Add("userInputFields",userInputFields);
				if (activityForm.Flow==null)
				{
				    return RedirectToAction("ActivityForm", "Form",
				                            new RouteValueDictionary() {{"flowId", activityForm.ProcessDefinition.Id}});
					//StartProcessInstance(activityForm.ProcessDefinition.Id);
				} 
                else
				{
				    return RedirectToAction("ActivityForm", "Form",new RouteValueDictionary());
					//ShowActivityForm(activityForm.Flow.Id);
				}
			} 
			else 
			{
				// remove the old inputvalues
				HttpContext.Session.Remove("userInputFields");
				//log.Debug( "submitting the form..." );
				IList activatedFlows = null;
				IFlow flow = activityForm.Flow;
				// if there is no flow in the activityForm
				IExecutionSessionLocal executionComponent = null;
				try
				{
					executionComponent = ServiceLocator.Instance.GetService(typeof (IExecutionSessionLocal)) as IExecutionSessionLocal;
					if ( flow == null ) 
					{
						// this means that it is a start-activity being performed so we have to 
						// start a new process instance
						IProcessDefinition processDefinition = activityForm.ProcessDefinition;
						IProcessInstance processInstance = executionComponent.StartProcessInstance( processDefinition.Id, userInputFields );
						activatedFlows = new ArrayList();
						//AddAllActiveFlows(processInstance.RootFlow,activatedFlows);
						//activatedFlows.Add(processInstance.RootFlow);
					} 
					else 
					{
						activatedFlows = executionComponent.PerformActivity( flow.Id, userInputFields );
					}
					
				}
				finally
				{
					ServiceLocator.Instance.Release(executionComponent);
				}
				if (activatedFlows.Count > 0)
				{
					System.Text.StringBuilder feedbackBuffer = new System.Text.StringBuilder();
					for(int i=0;i<activatedFlows.Count;++i)
					{
						IFlow activatedFlow = (IFlow) activatedFlows[i];
						
						if (activatedFlow.GetActor() != null)
						{
							feedbackBuffer.Append(activatedFlow.GetActor().Name);
						}
						else
						{
							// when flow's node is start-state no actor is assigned to it, this is to handle the NPE thrown 
							feedbackBuffer.Append("Nobody");
						}
						if (i+1<activatedFlows.Count)
							feedbackBuffer.Append(", ");
					}

					if (activatedFlows.Count > 1)
					{
						//AddMessage("Now, following people are handling this process :"+feedbackBuffer.ToString());
					}
					else
					{
						//AddMessage("Now, "+  feedbackBuffer.ToString() +" is handling this process");
					}
				    return Redirect("/User/ShowHome");
				}
				else
				{
					//AddMessage("This flow in the process finished");
                    return Redirect("/User/ShowHome");
				}
			}

		}

        private void AddFormData(IActivityForm activityForm)
        {
            IDictionary userInputFields = (IDictionary)HttpContext.Session["userInputFields"];
            if (userInputFields == null)
            {
                userInputFields = new Hashtable();
            }
            HttpContext.Session.Add("activityForm", activityForm);
            IList fields = activityForm.Fields;
            IEnumerator fildEnumer = fields.GetEnumerator();
            IList formRows = new ArrayList();
            while (fildEnumer.MoveNext())
            {
                IField field = (IField)fildEnumer.Current;
                // Construct a meaningfull name that is http-compliant
                String attributeName = field.Attribute.Name;
                String parameterName = convertToHttpCompliant(attributeName);

                IHtmlFormatter htmlFormatter = field.GetHtmlFormatter();
                if (htmlFormatter != null)
                {
                    Object objectToFormat = null;
                    if (userInputFields.Contains(attributeName))
                    {
                        objectToFormat = userInputFields[attributeName];
                    }
                    else
                    {
                        objectToFormat = activityForm.AttributeValues[field.Attribute.Name];
                    }
                    // TODO: Test if there is the possibility to simplify the interface, see null
                    String html = htmlFormatter.ObjectToHtml(objectToFormat, parameterName, null);
                    FormRow formRow = new FormRow(field, html);
                    formRows.Add(formRow);
                }
                else
                {
                    //log.Warn("skipping field for attribute '" + attributeName + "' because it doesn't have a HtmlFormatter");
                }
            }
            ViewData["formRows"] = formRows;
        }

        private String convertToHttpCompliant(String attributeName)
        {
            System.Text.StringBuilder parameterNameBuffer = new System.Text.StringBuilder();
            for (int i = 0; i < attributeName.Length; i++)
            {
                char c = attributeName[i];
                if (System.Char.IsLetterOrDigit(c))
                {
                    parameterNameBuffer.Append(c);
                }
                else
                {
                    parameterNameBuffer.Append('_');
                }
            }
            String parameterName = parameterNameBuffer.ToString();
            return parameterName;
        }

        private void AddAllActiveFlows(IFlow flow, IList flows)
        {
            if (flow.Children == null || flow.Children.Count == 0)
            {
                flows.Add(flow);
            }
            else
            {
                IEnumerator flowEnumer = flow.Children.GetEnumerator();
                while (flowEnumer.MoveNext())
                {
                    AddAllActiveFlows((IFlow)flowEnumer.Current, flows);
                }
            }
        }

    }
}
