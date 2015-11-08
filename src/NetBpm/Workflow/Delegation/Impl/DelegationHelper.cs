using System;
using System.Collections;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.ClassLoader;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Log.Impl;
using NHibernate.Type;

namespace NetBpm.Workflow.Delegation.Impl
{
	public class DelegationHelper
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DelegationHelper));

		public static DelegationHelper Instance
		{
			get { return instance; }
		}

		// first the singleton pattern
		private DelegationHelper()
		{
		}

		private static readonly DelegationHelper instance = new DelegationHelper();

		public void DelegateAction(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			try
			{
				executionContext.CreateLog(EventType.ACTION);
				executionContext.AddLogDetail(new DelegateCallImpl(delegation, typeof (IAction)));
				IActionHandler actionHandler = (IActionHandler) GetDelegate(delegation);
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				actionHandler.Run(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}
		}

		public void DelegateScheduledAction(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			try
			{
				/* can't add logs because of a integritiy constraint violation... 
				* can you find why ?
				*/
				if (executionContext.GetFlow() != null)
				{
					executionContext.CreateLog(EventType.ACTION);
					executionContext.AddLogDetail(new DelegateCallImpl(delegation, typeof (IAction)));
				}
				IActionHandler actionHandler = (IActionHandler) GetDelegate(delegation);
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				actionHandler.Run(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}
		}

		private const String queryFindLeavingTransitionByName = "select t " + "from t in class NetBpm.Workflow.Definition.Impl.TransitionImpl " +
			"where t.From.id = ? " + "  and t.Name = ? ";

		public TransitionImpl DelegateDecision(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			TransitionImpl selectedTransition = null;

			try
			{
				IDecisionHandler decision = (IDecisionHandler) GetDelegate(delegation);
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				String transitionName = decision.Decide(executionContext);

				if ((Object) transitionName == null)
				{
					throw new SystemException("Decision-delegate for decision '" + executionContext.GetNode() + "' returned null instead of a transition-name : " + decision.GetType().FullName);
				}

				try
				{
					Object[] args = new Object[] {executionContext.GetNode().Id, transitionName};
					IType[] types = new IType[] {DbType.LONG, DbType.STRING};
					selectedTransition = (TransitionImpl) executionContext.DbSession.FindOne(queryFindLeavingTransitionByName, args, types);
				}
				catch (Exception t)
				{
					throw new SystemException("couldn't find transition '" + transitionName + "' that was selected by the decision-delegate of activity '" + executionContext.GetNode().Name + "' : " + t.Message);
				}
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}

			return selectedTransition;
		}

		public Object[] DelegateProcessInvocation(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			Object[] invocationData = new Object[2];
			try
			{
				IProcessInvocationHandler processInvoker = (IProcessInvocationHandler) delegation.GetDelegate();
				log.Debug("requesting the attributeValues from the process invoker...");
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				invocationData[0] = processInvoker.GetStartTransitionName(executionContext);
				invocationData[1] = processInvoker.GetStartAttributeValues(executionContext);
				log.Debug("process invoker specified transition '" + invocationData[0] + "' and supplied attributeValues '" + invocationData[1] + "'");
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}
			return invocationData;
		}

		public Object[] DelegateProcessTermination(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			Object[] completionData = new Object[2];
			try
			{
				IProcessInvocationHandler processInvoker = (IProcessInvocationHandler) delegation.GetDelegate();
				log.Debug("collecting results from the sub-process...");
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				completionData[0] = processInvoker.CollectResults(executionContext);
				completionData[1] = processInvoker.GetCompletionTransitionName(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}
			return completionData;
		}

		public void DelegateFork(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			try
			{
				// delegate the fork
				IForkHandler forker = (IForkHandler) delegation.GetDelegate();
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				forker.Fork(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}
		}

		public bool DelegateJoin(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			bool reactivateParent = false;

			try
			{
				IJoinHandler joiner = (IJoinHandler) delegation.GetDelegate();
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				reactivateParent = joiner.Join(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}

			return reactivateParent;
		}

		public String DelegateAssignment(DelegationImpl delegation, ExecutionContextImpl executionContext)
		{
			String actorId = null;

			try
			{
				IAssignmentHandler assigner = (IAssignmentHandler) delegation.GetDelegate();
				executionContext.SetConfiguration(ParseConfiguration(delegation));
				actorId = assigner.SelectActor(executionContext);
			}
			catch (Exception t)
			{
				HandleException(delegation, executionContext, t);
			}

			return actorId;
		}


		private void HandleException(DelegationImpl delegation, ExecutionContextImpl executionContext, Exception exception)
		{
			log.Debug("handling delegation exception :", exception);

			String exceptionClassName = exception.GetType().FullName;
			String delegationClassName = delegation.ClassName;

			ExceptionHandlingType exceptionHandlingType = delegation.ExceptionHandlingType;

			if (exceptionHandlingType != 0)
			{
				if (exceptionHandlingType == ExceptionHandlingType.IGNORE)
				{
					log.Debug("ignoring '" + exceptionClassName + "' in delegation '" + delegationClassName + "' : " + exception.Message);
				}
				else if (exceptionHandlingType == ExceptionHandlingType.LOG)
				{
					log.Debug("logging '" + exceptionClassName + "' in delegation '" + delegationClassName + "' : " + exception.Message);
					executionContext.AddLogDetail(new ExceptionReportImpl(exception));
				}
				else if (exceptionHandlingType == ExceptionHandlingType.ROLLBACK)
				{
					log.Debug("rolling back for '" + exceptionClassName + "' in delegation '" + delegationClassName + "' : " + exception.Message);
					throw new SystemException("rolling back for '" + exceptionClassName + "' in delegation '" + delegationClassName + "' : " + exception.Message);
				}
				else
				{
					throw new SystemException("unknown exception handler '" + exceptionHandlingType + "' : " + exception.Message);
				}
			}
			else
			{
				log.Debug("'" + exceptionClassName + "' in delegation '" + delegationClassName + "' : " + exception.Message);
				executionContext.AddLogDetail(new ExceptionReportImpl(exception));
			}
		}

		public Object GetDelegate(DelegationImpl delegationImpl)
		{
			Object delegateClass = null;
			IClassLoader classLoader = null;
			try
			{
				classLoader = (IClassLoader) ServiceLocator.Instance.GetService(typeof (IClassLoader));
				delegateClass = classLoader.CreateObject(delegationImpl);
			}
			finally
			{
				ServiceLocator.Instance.Release(classLoader);
			}
			// configure class
			if (delegateClass is IConfigurable)
			{
				IConfigurable configurable = (IConfigurable) delegateClass;
				IDictionary parameters = ParseConfiguration(delegationImpl);
				configurable.SetConfiguration(parameters);
			}
			return delegateClass;
		}

		private IDictionary ParseConfiguration(DelegationImpl delegationImpl)
		{
			IDictionary parameters = new Hashtable();
			try
			{
				String configuration = delegationImpl.Configuration;
				if ((Object) configuration != null)
				{
					XmlParser xmlParser = new XmlParser(configuration);
					xmlParser.Validation = false;
					XmlElement configurationXmlElement = xmlParser.Parse();
					IList parameterXmlElements = configurationXmlElement.GetChildElements("parameter");
					IEnumerator iter = parameterXmlElements.GetEnumerator();
					while (iter.MoveNext())
					{
						XmlElement parameterXmlElement = (XmlElement) iter.Current;

						String name = parameterXmlElement.GetProperty("name");
						if ((Object) name == null)
						{
							throw new SystemException("invalid delegation-configuration : " + configurationXmlElement);
						}

						parameters[name] = GetObject(parameterXmlElement);
					}
				}
			}
			catch (Exception t)
			{
				log.Error("can't parse configuration : ", t);
				throw new SystemException("can't parse configuration : " + t.Message);
			}

			return parameters;
		}

		private Object GetObject(XmlElement xmlElement)
		{
			Object object_Renamed = null;

			String className = xmlElement.GetProperty("class");

			if (((Object) className == null) || ("java.lang.String".Equals(className)))
			{
				object_Renamed = GetText(xmlElement);
			}
			else if ("java.util.List".Equals(className))
			{
				object_Renamed = GetList(xmlElement);
			}
			else if ("java.util.Map".Equals(className))
			{
				object_Renamed = GetMap(xmlElement);
			}
			else
			{
				log.Error("Error getting object->@portme");
				//@portme
/*				try
				{
					Type clazz = Type.GetType(className);
					System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(constructorArgumentTypes);
					object_Renamed = constructor.newInstance(new Object[]{xmlElement.getContentString()});
				}
				catch (System.Exception t)
				{
					log.Error("Error getting object", t);
				}*/
			}
			return object_Renamed;
		}

		private String GetText(XmlElement xmlElement)
		{
			return xmlElement.GetContentString().Trim();
		}

		private IList GetList(XmlElement xmlElement)
		{
			IList list = new ArrayList();

			IList elementXmlElements = xmlElement.GetChildElements("element");
			IEnumerator iter = elementXmlElements.GetEnumerator();
			while (iter.MoveNext())
			{
				list.Add(GetObject((XmlElement) iter.Current));
			}

			return list;
		}

		private IDictionary GetMap(XmlElement xmlElement)
		{
			IDictionary map = new Hashtable();

			IList elementXmlElements = xmlElement.GetChildElements("entry");
			IEnumerator iter = elementXmlElements.GetEnumerator();
			while (iter.MoveNext())
			{
				XmlElement entryXmlElement = (XmlElement) iter.Current;

				// get the key      
				XmlElement key = entryXmlElement.GetChildElement("key");
				if (key == null)
					throw new SystemException("an <entry> must contain exactly one <key> sub-element");

				// get the value
				XmlElement valueObject = entryXmlElement.GetChildElement("value");
				if (valueObject == null)
					throw new SystemException("an <entry> must contain exactly one <value> sub-element");

				map[GetObject(key)] = GetObject(valueObject);
			}

			return map;
		}
	}
}