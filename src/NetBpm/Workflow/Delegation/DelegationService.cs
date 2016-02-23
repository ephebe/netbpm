using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.ClassLoader;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution.Impl;
using NHibernate.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Delegation
{
    public class DelegationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DelegationService));
        private static readonly DelegationHelper delegationHelper = DelegationHelper.Instance;

        private const String queryFindActionsByEventType = "select a from a in class NetBpm.Workflow.Definition.Impl.ActionImpl " +
            "where a.EventType = ? " +
            "  and a.DefinitionObjectId = ? ";

        private ActorExpressionResolverService actorExpressionResolverService = null;

        public DelegationService() 
        {
        
        }

        //public String DelegateAssignment(DelegationImpl delegation)
        //{
        //    String actorId = null;

        //    IAssignmentHandler assigner = (IAssignmentHandler)delegation.GetDelegate();
        //    IDictionary expression = this.ParseConfiguration(delegation);

        //    actorExpressionResolverService = new ActorExpressionResolverService();
        //    actorId = actorExpressionResolverService.ResolveArgument(expression["expression"].ToString()).Id;            

        //    return actorId;
        //}

        public Object GetDelegate(DelegationImpl delegationImpl)
        {
            Object delegateClass = null;
            IClassLoader classLoader = null;
            try
            {
                classLoader = (IClassLoader)ServiceLocator.Instance.GetService(typeof(IClassLoader));
                delegateClass = classLoader.CreateObject(delegationImpl);
            }
            finally
            {
                ServiceLocator.Instance.Release(classLoader);
            }
            // configure class
            if (delegateClass is IConfigurable)
            {
                IConfigurable configurable = (IConfigurable)delegateClass;
                IDictionary parameters = ParseConfiguration(delegationImpl);
                configurable.SetConfiguration(parameters);
            }
            return delegateClass;
        }

        public IDictionary ParseConfiguration(DelegationImpl delegationImpl)
        {
            IDictionary parameters = new Hashtable();
            try
            {
                String configuration = delegationImpl.Configuration;
                if ((Object)configuration != null)
                {
                    XmlParser xmlParser = new XmlParser(configuration);
                    xmlParser.Validation = false;
                    XmlElement configurationXmlElement = xmlParser.Parse();
                    IList parameterXmlElements = configurationXmlElement.GetChildElements("parameter");
                    IEnumerator iter = parameterXmlElements.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        XmlElement parameterXmlElement = (XmlElement)iter.Current;

                        String name = parameterXmlElement.GetProperty("name");
                        if ((Object)name == null)
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

            if (((Object)className == null) || ("java.lang.String".Equals(className)))
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
                list.Add(GetObject((XmlElement)iter.Current));
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
                XmlElement entryXmlElement = (XmlElement)iter.Current;

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

        public void RunActionsForEvent(EventType eventType, long definitionObjectId, ExecutionContextImpl executionContext,DbSession dbSession)
        {
            log.Debug("processing '" + eventType + "' events for executionContext " + executionContext);

            // find all actions for definitionObject on the given eventType
            Object[] values = new Object[] { eventType, definitionObjectId };
            IType[] types = new IType[] { DbType.INTEGER, DbType.LONG };

            IList actions = dbSession.Find(queryFindActionsByEventType, values, types);
            IEnumerator iter = actions.GetEnumerator();
            log.Debug("list" + actions);
            while (iter.MoveNext())
            {
                ActionImpl action = (ActionImpl)iter.Current;
                log.Debug("action: " + action);
                delegationHelper.DelegateAction(action.ActionDelegation, executionContext);
            }
            log.Debug("ende runActionsForEvent!");
        }
    }
}
