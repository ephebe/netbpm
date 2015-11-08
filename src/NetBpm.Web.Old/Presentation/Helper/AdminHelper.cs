using System;
using System.Collections;
using System.IO;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Log;
using NetBpm.Workflow.Log.EComp;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Web.Presentation.Helper
{
	public class AdminHelper
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (AdminHelper));
		protected ILogSessionLocal logComponent = null;
		private ArrayList details = null;

		public String CreateLogDetail(Int64 flowId)
		{
			logComponent = ServiceLocator.Instance.GetService(typeof (ILogSessionLocal)) as ILogSessionLocal;
			try 
			{
				details = new ArrayList();
				StringWriter logDetail=new StringWriter();
				IFlow flow = GetFlow(flowId);
				WriteFlow(logDetail, flow);
				return logDetail.ToString();
			} 
			finally
			{
				ServiceLocator.Instance.Release(logComponent);
			}
		}

		private void WriteFlow(TextWriter logDetail, IFlow flow)
		{
			WriteFlowStart(logDetail, flow);
			IEnumerator iter = flow.Logs.GetEnumerator();
			//@todo add sort see http://www.koders.com/csharp/fid7483E2E4A14A57A929B3D9338410A60E4E911842.aspx?s=sort
			while (iter.MoveNext())
			{
				ILog eventLog = (ILog) iter.Current;
				WriteFlowEvent(logDetail, eventLog, flow);
			}
			WriteFlowEnd(logDetail, flow);
		}

		private void  WriteFlowStart(TextWriter logDetail, IFlow flow)
		{
			logDetail.Write("<table border=0 cellpadding=0 cellspacing=1 bgcolor=#000000 height=100%><tr><td bgcolor=#ffffff height=100% valign=top>" + "\r\n");
			logDetail.Write("<table border=0 cellpadding=0 cellspacing=0>" + "\r\n");
			logDetail.Write("<tr><th class=tableFlowCell width=100% colspan=2 nowrap> flow [<b>" + flow.Name + "</b>]</th></tr>" + "\r\n");
		}
		
		private void  WriteFlowEnd(TextWriter logDetail, IFlow flow)
		{
			logDetail.Write("</table>" + "\r\n");
			logDetail.Write("</td></tr></table>" + "\r\n");
		}

		private void  WriteFlowEvent(TextWriter logDetail, ILog eventLog, IFlow flow)
		{
			logDetail.Write("  <tr>" + "\r\n");
			logDetail.Write("    <td bgcolor=#ffffff>" + "\r\n");
			logDetail.Write("      <table border=0 cellspacing=0 cellpadding=0 width=100%>" + "\r\n");
			logDetail.Write("        <tr><th class=tableEventLogCell width=100% colspan=2 nowrap>" + eventLog.EventType + "</th></tr>" + "\r\n");
			logDetail.Write("        <tr>" + "\r\n");
			logDetail.Write("          <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>" + "\r\n");
			logDetail.Write("          <td width=100%>" + "\r\n");
			
			this.details = new System.Collections.ArrayList();
			AddDetail("Time", eventLog.Date.ToString());
			
			IActor actor = eventLog.GetActor();
			if (actor != null)
			{
				AddDetail("Actor", actor.Name);
			}
			
			if (eventLog.EventType == EventType.FORK)
			{
				logDetail.Write("            <table border=0 cellpadding=0 cellspacing=5>" + "\r\n");
				logDetail.Write("              <tr>" + "\r\n");
				IEnumerator iter = eventLog.GetObjectReferences("Flow").GetEnumerator();
			
				while (iter.MoveNext())
				{
					IObjectReference objectReference = (IObjectReference) iter.Current;
					IFlow subFlow = (IFlow) objectReference.GetObject();
					subFlow = GetFlow(subFlow.Id);
					
					logDetail.Write("                <td valign=top height=100%>" + "\r\n");
					WriteFlow(logDetail, subFlow);
					logDetail.Write("                </td>" + "\r\n");
				}
				logDetail.Write("              </tr>" + "\r\n");
				logDetail.Write("            </table>" + "\r\n");
			}
			else if (eventLog.EventType == EventType.SUB_PROCESS_INSTANCE_START)
			{
				IObjectReference objectReference = (IObjectReference) eventLog.GetObjectReferences("ProcessInstance").GetEnumerator().Current;
				IProcessInstance subProcessInstance = (IProcessInstance) objectReference.GetObject();
				IFlow subFlow = GetFlow(subProcessInstance.RootFlow.Id);
				WriteFlow(logDetail, subFlow);
			}
			else
			{
				// if it is no fork or subprocess, log the details
				IEnumerator iter = eventLog.Details.GetEnumerator();
				while (iter.MoveNext())
				{
					AddDetail((ILogDetail) iter.Current);
				}
				
				WriteDetails(logDetail);
			}
			
			logDetail.Write("          </td>" + "\r\n");
			logDetail.Write("        </tr>" + "\r\n");
			logDetail.Write("        <tr><td>&nbsp;</td></tr>" + "\r\n");
			logDetail.Write("      </table>" + "\r\n");
			logDetail.Write("    </td>" + "\r\n");
			logDetail.Write("  </tr>" + "\r\n");
		}

		private void  AddDetail(System.String key, System.String valueObject)
		{
			details.Add(new System.String[]{key, valueObject});
		}
		
		private void  AddDetail(ILogDetail logDetail)
		{
			if (logDetail is IAttributeUpdate)
			{
				IAttributeUpdate attributeUpdate = (IAttributeUpdate) logDetail;
				AddDetail("Attribute update", "[" + attributeUpdate.Attribute.Name + "]</b> to <b>[" + attributeUpdate.GetValue() + "]");
			}
			else if (logDetail is IMessage)
			{
				IMessage message = (IMessage) logDetail;
				AddDetail("Message", message.MessageText);
			}
			else if (logDetail is IDelegateCall)
			{
				IDelegateCall delegateCall = (IDelegateCall) logDetail;
				// FIXME: why this could be null?
				if (delegateCall.GetInterface() != null)
				{
					AddDetail("Action handler", delegateCall.GetInterface().FullName);
				}
			}
			else if (logDetail is IExceptionReport)
			{
				IExceptionReport exceptionReport = (IExceptionReport) logDetail;
				AddDetail("<font color=red>Exception</font>", "<font color=red>" + exceptionReport.ExceptionMessage + "</font>");
			}
			else if (logDetail is IObjectReference)
			{
				IObjectReference objectReference = (IObjectReference) logDetail;
				System.Object object_Renamed = objectReference.GetObject();
				if (object_Renamed is IActivityState)
				{
					IActivityState activityState = (IActivityState) object_Renamed;
					AddDetail("Activitystate", activityState.Name);
				}
				else
				{
					log.Warn("unknown object reference type : " + object_Renamed);
				}
			}
			else
			{
				log.Warn("unknown log-detail type : " + logDetail);
			}
		}
		
		private void  WriteDetails(TextWriter logDetail)
		{
			if (details.Count > 0)
			{
				logDetail.Write("            <table class=table border=0 cellspacing=1 cellpadding=2>" + "\r\n");
				System.Collections.IEnumerator iter = details.GetEnumerator();
				while (iter.MoveNext())
				{
					System.String[] detail = (System.String[]) iter.Current;
					logDetail.Write("              <tr class=tableRowEven>" + "\r\n");
					logDetail.Write("                <td nowrap class=tableCell valign=top>" + detail[0] + "</td>" + "\r\n");
					logDetail.Write("                <td class=tableCell valign=top><b>" + detail[1] + "</b></td>" + "\r\n");
					logDetail.Write("              </tr>" + "\r\n");
				}
				logDetail.Write("            </table>" + "\r\n");
			}
		}

		private IFlow GetFlow(Int64 flowId)
		{
			IFlow flow = null;
			flow = logComponent.GetFlow(flowId);
			return flow;
		}
	}
}
