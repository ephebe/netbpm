using System;
using System.Collections;
using log4net;
using Castle.MonoRail.Framework;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Util.Client;
using NetBpm.Web.Presentation.Model;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	public class FormController : AbstractSecureController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FormController));

		public FormController()
		{
		}

		public void StartProcessInstance(long processDefinitionId)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("StartProcessInstance processDefinitionId:"+processDefinitionId);
			}

			IExecutionSessionLocal executionComponent = null;
			IDefinitionSessionLocal definitionComponent = null;
			try
			{
				executionComponent = ServiceLocator.Instance.GetService(typeof (IExecutionSessionLocal)) as IExecutionSessionLocal;
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IDefinitionSessionLocal)) as IDefinitionSessionLocal;
				IProcessDefinition processDefinition = definitionComponent.GetProcessDefinition(processDefinitionId);

				Context.Flash["activity"] = processDefinition.StartState;
				Context.Flash["processDefinition"] = processDefinition;
				Context.Flash["preview"] = "activity";
				AddImageCoordinates(processDefinition.StartState);

				//create Form
				IActivityForm activityForm = executionComponent.GetStartForm(processDefinitionId);
				AddFormData(activityForm);
				RenderView("activityForm");
			}
			finally
			{
				ServiceLocator.Instance.Release(executionComponent);
				ServiceLocator.Instance.Release(definitionComponent);
			}
		}

		public void ShowActivityForm(long flowId)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("ShowActivityForm flowId:"+flowId);
			}
			IExecutionSessionLocal executionComponent = null;
			try
			{
				executionComponent = ServiceLocator.Instance.GetService(typeof (IExecutionSessionLocal)) as IExecutionSessionLocal;
				IFlow flow = executionComponent.GetFlow(flowId);

				Context.Flash["activity"] = flow.Node;
				Context.Flash["processDefinition"] = flow.ProcessInstance.ProcessDefinition;
				Context.Flash["preview"] = "activity";
				AddImageCoordinates((IState)flow.Node);

				//create Form
				IActivityForm activityForm = executionComponent.GetActivityForm(flowId);
				AddFormData(activityForm);
			}
			finally
			{
				ServiceLocator.Instance.Release(executionComponent);
			}

			RenderView("activityForm");
		}

		public void SubmitActivityForm()
		{
			IDictionary userInputFields = new Hashtable();
			IActivityForm activityForm = (IActivityForm)Context.Session["activityForm"];
			IList fields = activityForm.Fields;
			IEnumerator fildEnumer = fields.GetEnumerator();
			while (fildEnumer.MoveNext())
			{
				IField field = (IField)fildEnumer.Current;
				// Construct a meaningfull name that is http-compliant
				String attributeName = field.Attribute.Name;
				String parameterName = convertToHttpCompliant(attributeName);
				String parameterValue = Context.Request.Params[parameterName];

				if (FieldAccessHelper.IsRequired(field.Access) && (parameterValue==null || "".Equals(parameterValue)))
				{
					AddMessage("Field "+field.Name+" is required. Please, provide a value");
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
								log.Debug( "setting field " + attributeName + " to value " + parsedParameter );
								userInputFields.Add( attributeName, parsedParameter );
							}				
						} 
						else
						{
							log.Warn("No htmlformatter defined for field:"+field.Name);
						}
					} 
					catch (Exception ex)
					{
						log.Debug( "error parsing user-input-field " + field.Name + " : " + parameterValue,ex);
						AddMessage("error parsing user-input-field " + field.Name + " with value: " + parameterValue);
					}
				}
			}

			if ( HasMessages() ) 
			{
				log.Debug( "submitted activity-form has messages, redirecting to activityFormPage..." );
				Context.Session.Add("userInputFields",userInputFields);
				if (activityForm.Flow==null)
				{
					StartProcessInstance(activityForm.ProcessDefinition.Id);
				} else {
					ShowActivityForm(activityForm.Flow.Id);
				}
			} 
			else 
			{
				// remove the old inputvalues
				Context.Session.Remove("userInputFields");
				log.Debug( "submitting the form..." );
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
						AddAllActiveFlows(processInstance.RootFlow,activatedFlows);
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
						AddMessage("Now, following people are handling this process :"+feedbackBuffer.ToString());
					}
					else
					{
						AddMessage("Now, "+  feedbackBuffer.ToString() +" is handling this process");
					}
					Redirect("user","showHome");
				}
				else
				{
					AddMessage("This flow in the process finished");
					Redirect("user","showHome");
				}
			}
		}

		private void AddFormData(IActivityForm activityForm) {
			IDictionary userInputFields =  (IDictionary)Context.Session["userInputFields"];
			if (userInputFields == null)
			{
				userInputFields = new Hashtable();
			}
			Context.Session.Add("activityForm",activityForm);
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
					String html = htmlFormatter.ObjectToHtml( objectToFormat, parameterName, null);
					FormRow formRow = new FormRow(field,html);
					formRows.Add(formRow);
				} 
				else 
				{
					log.Warn( "skipping field for attribute '" + attributeName + "' because it doesn't have a HtmlFormatter" );
				}
			}
			Context.Flash["formRows"] = formRows;
		}
	

		private String convertToHttpCompliant(String attributeName) {
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
					AddAllActiveFlows((IFlow)flowEnumer.Current,flows);
				}
			}
		}
	}
}
