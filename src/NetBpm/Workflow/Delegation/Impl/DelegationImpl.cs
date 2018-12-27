using System;
using System.Collections;
using log4net;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;

namespace NetBpm.Workflow.Delegation.Impl
{
	/// <summary> manages all information for one delegation.</summary>
	public class DelegationImpl : IDelegation
	{
		private Int64 _id;
		private IProcessDefinition _processDefinition = null;
		private String _className = null;
		private String _configuration = null;
		private ExceptionHandlingType _exceptionHandlingType = 0;

		public static readonly IDictionary attributeTypes = new Hashtable();
		private static readonly ILog log = LogManager.GetLogger(typeof (DelegationImpl));

		static DelegationImpl()
		{
			attributeTypes["actor"] = "NetBpm.Workflow.Delegation.Impl.Serializer.ActorSerializer";
			attributeTypes["text"] = "NetBpm.Workflow.Delegation.Impl.Serializer.TextSerializer";
			attributeTypes["long"] = "NetBpm.Workflow.Delegation.Impl.Serializer.LongSerializer";
			attributeTypes["integer"] = "NetBpm.Workflow.Delegation.Impl.Serializer.IntegerSerializer";
			attributeTypes["float"] = "NetBpm.Workflow.Delegation.Impl.Serializer.FloatSerializer";
			attributeTypes["date"] = "NetBpm.Workflow.Delegation.Impl.Serializer.DateSerializer";
			attributeTypes["evaluation"] = "NetBpm.Workflow.Delegation.Impl.Serializer.EvaluationSerializer";
		}

		public virtual Int64 Id
		{
			set { _id = value; }
			get { return _id; }
		}

        public virtual IProcessDefinition ProcessDefinition
		{
			set { _processDefinition = value; }
			get { return _processDefinition; }
		}

        public virtual String ClassName
		{
			set { _className = value; }
			get { return _className; }
		}

        public virtual String Configuration
		{
			set { _configuration = value; }
			get { return _configuration; }
		}

        public virtual ExceptionHandlingType ExceptionHandlingType
		{
			set { _exceptionHandlingType = value; }
			get { return _exceptionHandlingType; }
		}

		public DelegationImpl()
		{
		}

		public DelegationImpl(IProcessDefinition processDefinition)
		{
			this._processDefinition = processDefinition;
		}

        public virtual void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			this._processDefinition = creationContext.ProcessDefinition;

			Type delegatingObjectClass = creationContext.DelegatingObject.GetType();
			if (delegatingObjectClass == typeof (AttributeImpl))
			{
				String type = xmlElement.GetProperty("type");
				if ((Object) type != null)
				{
					this._className = ((String) attributeTypes[type]);
					string suportedTypes = "supported types: ";
					foreach (Object o in attributeTypes.Keys)
					{
						suportedTypes += o.ToString() + " ,";				
					}
					creationContext.Check(((Object) this._className != null), "attribute type '" + type + "' is not supported. " + suportedTypes +" !");
				}
				else
				{
					this._className = xmlElement.GetProperty("serializer");
					creationContext.Check(((Object) this._className != null), "for an attribute, you must specify either a type or a serializer");
				}
			}
			else if (delegatingObjectClass == typeof (FieldImpl))
			{
				this._className = xmlElement.GetProperty("class");
				creationContext.Check(((Object) this._className != null), "no class specified for a delegation : " + xmlElement);
			}
			else
			{
				this._className = xmlElement.GetProperty("handler");
				creationContext.Check(((Object) this._className != null), "no handler specified for a delegation : " + xmlElement);
			}

			log.Debug("parsing delegation for tag '" + xmlElement.Name + "' : " + this._className);

			// parse the exception handler    
			String exceptionHandlerText = xmlElement.GetAttribute("on-exception");
			if ((Object) exceptionHandlerText != null)
			{
				_exceptionHandlingType = ExceptionHandlingTypeHelper.FromText(exceptionHandlerText);
				creationContext.Check((_exceptionHandlingType != 0), "unknown exception handler '" + exceptionHandlerText + "' in delegation " + xmlElement);
			}

			// create the configuration string
			XmlElement configurationXml = new XmlElement("cfg");
			IEnumerator iter = xmlElement.GetChildElements("parameter").GetEnumerator();
			while (iter.MoveNext())
			{
				configurationXml.AddChild((XmlElement) iter.Current);
			}
			_configuration = configurationXml.ToString();
		}

        public virtual IDictionary ParseConfiguration()
        {
            IDictionary parameters = new Hashtable();
            try
            {
                String configuration = this.Configuration;
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

		public override String ToString()
		{
			return "delegation[" + _id + "|" + _className + "|" + _processDefinition.Id + "]";
		}

        public virtual Object GetDelegate()
		{
			return DelegationHelper.Instance.GetDelegate(this);
		}
	}
}