using System;
using System.Collections;
using System.Globalization;
using Castle.MonoRail.Framework;
using NetBpm.Util.Client;
using NetBpm.Web.Presentation.Helper;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Log.EComp;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	[Helper( typeof(AdminHelper) )]
	public class AdminController : AbstractSecureController
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (AdminController));
		private static readonly String DATEFORMAT = "dd/MM/yyyy";
		private static readonly CultureInfo enUS = new CultureInfo("en-US", false);

		public void StartScheduler()
		{
			ServiceLocator.Instance.Scheduler.Start();
			Redirect("admin","showHome");
		}

		public void StopScheduler()
		{
			ServiceLocator.Instance.Scheduler.Stop();
			Redirect("admin","showHome");
		}

		public void ShowHome(Int32 processDefinitionId,String startAfter,
							 String startBefore,String initiator,String actor)
		{

			IDefinitionSessionLocal definitionComponent = null;
			ILogSessionLocal logComponent = null;
			try
			{
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IDefinitionSessionLocal)) as IDefinitionSessionLocal;
				logComponent = ServiceLocator.Instance.GetService(typeof (ILogSessionLocal)) as ILogSessionLocal;
				if (log.IsDebugEnabled)
				{
					log.Debug("ShowHome processDefinitionId:"+processDefinitionId+" startAfter:"+startAfter+
						" startBefore:"+startBefore+" initiator:"+initiator+" actor:"+actor);
				}
				Context.Flash["allProcessDefinitions"] = definitionComponent.GetAllProcessDefinitions();
				// date text to datetime
				DateTime before = DateTime.MinValue;
				DateTime after = DateTime.MinValue;
				if (startAfter != null && startAfter != "")
				{
					try
					{
						after = DateTime.ParseExact(startAfter, DATEFORMAT ,enUS);
					}
					catch (FormatException ex)
					{
						AddMessage(startAfter+" is not a vaild dateformat!");
						log.Debug(startAfter+" is not a vaild dateformat!"+ex.Message);
					}
				}
				if (startBefore != null && startBefore != "")
				{
					try
					{
						before = DateTime.ParseExact(startBefore, DATEFORMAT ,enUS);
					}
					catch (FormatException ex)
					{
						AddMessage(startBefore+" is not a vaild dateformat!");
						log.Debug(startBefore+" is not a vaild dateformat!"+ex.Message);
					}
				}
			
				//show processes only if a definition is selected
				if (processDefinitionId != 0)
				{
					// TODO: add the handling of startAfter and startBefore 
					IList allProcessInstances = logComponent.FindProcessInstances( after,before,
						initiator,actor,processDefinitionId);
					if (allProcessInstances.Count != 0)
					{
						Context.Flash["allProcessInstances"] = allProcessInstances;
					}
				}
				Context.Flash["processDefinitionId"] = processDefinitionId;
				Context.Flash["initiator"] = initiator;
				Context.Flash["actor"] = actor;

				Context.Flash["startBefore"] = startBefore;
				Context.Flash["startAfter"] = startAfter;
				Context.Flash["serviceLocator"] = ServiceLocator.Instance;
			}
			finally
			{
				ServiceLocator.Instance.Release(logComponent);
				ServiceLocator.Instance.Release(definitionComponent);
			}
		}

		public void ShowProcessInstance(Int32 processInstanceId)
		{
			ILogSessionLocal logComponent = null;
			try
			{
				logComponent = ServiceLocator.Instance.GetService(typeof (ILogSessionLocal)) as ILogSessionLocal;
				IProcessInstance processInstance = logComponent.GetProcessInstance(processInstanceId);
				
				IList state = new ArrayList();
				IList attributeRows = new ArrayList();

				GetStateAndAttributes(state, attributeRows, "", processInstance.RootFlow);

				Context.Flash["processInstance"] = processInstance;
				Context.Flash["processInstanceStates"] = state;
				Context.Flash["attributeRows"] = attributeRows;
			}
			finally
			{
				ServiceLocator.Instance.Release(logComponent);
			}
		}

		private void GetStateAndAttributes(IList state, IList attributeRows, String indentation, IFlow flow)
		{
			// add the flow to the state
			IEnumerator iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				state.Add(iter.Current);
			}
			
			// add the flow-name to the attributeRows
			System.Collections.IDictionary row = new System.Collections.Hashtable();
			if (flow.IsRootFlow())
			{
				row["name"] = indentation + "rootflow <b>[</b>" + flow.Name + "<b>]</b>";
			}
			else
			{
				row["name"] = indentation + "subflow <b>[</b>" + flow.Name + "<b>]</b>";
			}
			row["value"] = "";
			attributeRows.Add(row);
			
			// add the flow-local attributes to the attributeRows
			iter = flow.AttributeInstances.GetEnumerator();
			while (iter.MoveNext())
			{
				IAttributeInstance attributeInstance = (IAttributeInstance) iter.Current;
				row = new Hashtable();
				
				log.Debug("adding attribute instance value " + attributeInstance.GetValue());
				row["name"] = indentation + "&nbsp;&nbsp;&nbsp;+-&nbsp;<b>[</b>" + 
							  attributeInstance.Attribute.Name + "<b>]</b>";
				row["value"] = attributeInstance.GetValue();
				attributeRows.Add(row);
			}
			
			// recursively descend to the children
			iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				GetStateAndAttributes(state, attributeRows, indentation + "&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;", (IFlow) iter.Current);
			}
			
			IProcessInstance processInstance = flow.GetSubProcessInstance();
			if (processInstance != null)
			{
				state.Add(processInstance.RootFlow);
				GetStateAndAttributes(state, attributeRows, indentation + "&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;", processInstance.RootFlow);
			}
		}

	}
}