using System;
using System.Collections;
using System.IO;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Log;
using NetBpm.Workflow.Log.EComp;
using NetBpm.Workflow.Log.Impl;

namespace NetBpm.Ext.NAnt.EComp.Impl
{
	[Transactional]
	public class PPMEComp  : AbstractEComp, IPPMSessionLocal
	{
		private ILogSessionLocal logComponent = null;

		public PPMEComp(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void ExportPPMFile(String exportPath)
		{
			try 
			{
				logComponent = ServiceLocator.Instance.GetService(typeof (ILogSessionLocal)) as ILogSessionLocal;
				if (logComponent==null)
				{
					throw new ArgumentException("Can’t create log component. Container is not configured please check the configfile");
				}
	
				IList processList = logComponent.FindProcessInstances(DateTime.MinValue,DateTime.MinValue,null,null,0);
				IEnumerator processEnum=processList.GetEnumerator();
				if (!Directory.Exists (exportPath))
					throw new FileNotFoundException (String.Format ("The directory `{0}' does not exist", exportPath));
				//create File
				FileInfo exportFile = new FileInfo(exportPath+"/"+DateTime.Today.ToString("yyyyMMddhhmm")+".xml");
				FileStream fstream = exportFile.OpenWrite();
				StreamWriter xmlwriter = new StreamWriter(fstream);
				xmlwriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				xmlwriter.WriteLine("<!DOCTYPE graphlist SYSTEM \"graph.dtd\">");
				xmlwriter.WriteLine("<graphlist>");
	
				while (processEnum.MoveNext())
				{
					IProcessInstance processInstance=(IProcessInstance)processEnum.Current;
					WriteProcess(xmlwriter, processInstance);

				}
	
				xmlwriter.WriteLine("</graphlist>");
				xmlwriter.Flush();
				xmlwriter.Close();
			} 
			finally
			{
				ServiceLocator.Instance.Release(logComponent);
			}
		}

		private void WriteProcess(StreamWriter xmlwriter, IProcessInstance processInstance)
		{
			xmlwriter.WriteLine("<!--");
			xmlwriter.WriteLine("ProcessDefinition.Name: "+processInstance.ProcessDefinition.Name);
			xmlwriter.WriteLine("ProcessDefinition.Version: "+processInstance.ProcessDefinition.Version);
			xmlwriter.WriteLine("-->");
			xmlwriter.WriteLine("	<graph id=\"" +processInstance.ProcessDefinition.Name + " - " +
				processInstance.RootFlow.Id+"\" xml:lang=\"en\">");
			WriterAttributes(processInstance.RootFlow,xmlwriter);
			WriterNodes(processInstance.RootFlow,xmlwriter);
			
			xmlwriter.WriteLine("	</graph>");
		}

		private void WriterAttributes(IFlow flow, StreamWriter xmlwriter)
		{
			//NHibernate.LazyInitializationException: Failed to lazily initialize a collection - no session
			IEnumerator iter = flow.AttributeInstances.GetEnumerator();
			while (iter.MoveNext())
			{
				IAttributeInstance attributeInstance = (IAttributeInstance)iter.Current;
				xmlwriter.WriteLine("		<attribute type=\"AT_UDA_12_"+attributeInstance.Attribute.Name+"\">"+attributeInstance.GetValue()+"</attribute>");
			}

			// recursively descend to the children
			iter = flow.Children.GetEnumerator();
			while (iter.MoveNext())
			{
				WriterAttributes((IFlow) iter.Current,xmlwriter);
			}
		}

		private void WriterNodes(IFlow flow, StreamWriter xmlwriter)
		{
			IEnumerator iter = flow.Logs.GetEnumerator();
			while (iter.MoveNext())
			{
				ILog eventLog = (ILog) iter.Current;
				xmlwriter.WriteLine("id->"+eventLog.Id);
				if (eventLog.EventType == EventType.FORK)
				{
					IEnumerator flowIter = eventLog.GetObjectReferences("Flow").GetEnumerator();
			
					while (flowIter.MoveNext())
					{
						ObjectReferenceImpl objectReference = (ObjectReferenceImpl) flowIter.Current;
						DbSession session = OpenSession();
						objectReference.Resolve(session);
						session.Close();
						IFlow subFlow = (IFlow) objectReference.GetObject();
						xmlwriter.WriteLine("subflow"+subFlow+" ->"+subFlow);
						WriterNodes(subFlow ,xmlwriter);
					}
				}			
				else if (eventLog.EventType == EventType.SUB_PROCESS_INSTANCE_START)
				{
					IObjectReference objectReference = (IObjectReference) eventLog.GetObjectReferences("ProcessInstance").GetEnumerator().Current;
					IProcessInstance subProcessInstance = (IProcessInstance) objectReference.GetObject();
					WriterNodes(subProcessInstance.RootFlow ,xmlwriter);
				}

			}
		}
	}
}
