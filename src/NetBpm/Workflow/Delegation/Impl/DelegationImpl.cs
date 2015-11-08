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