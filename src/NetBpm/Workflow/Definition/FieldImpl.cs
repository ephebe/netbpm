using System;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class FieldImpl : PersistentObject, IField
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FieldImpl));
		private String _name = null;
		private String _description = null;
		private Int32 _index;
		private IState _state = null;
		private IAttribute _attribute = null;
		private FieldAccess _access;
		[NonSerialized()] private DelegationImpl _htmlFormatterDelegation = null;
		private IHtmlFormatter _htmlFormatter = null;

        public virtual Int32 Index
		{
			get { return this._index; }
			set { this._index = value; }
		}

        public virtual DelegationImpl HtmlFormatterDelegation
		{
			get { return this._htmlFormatterDelegation; }
			set { this._htmlFormatterDelegation = value; }
		}

        public virtual String Name
		{
			set { _name = value; }
			get { return _name; }
		}

        public virtual String Description
		{
			set { _description = value; }
			get { return _description; }
		}

        public virtual IAttribute Attribute
		{
			set { _attribute = value; }
			get { return _attribute; }
		}

        public virtual IState State
		{
			set { _state = value; }
			get { return _state; }
		}

        public virtual FieldAccess Access
		{
			set { _access = value; }
			get { return _access; }
		}

		public FieldImpl()
		{
		}

        public virtual void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			String attributeName = xmlElement.GetProperty("attribute");
			creationContext.Check(((Object) attributeName != null), "attribute is a required property in element field : " + xmlElement);

			log.Debug("parsing field for attribute '" + attributeName);

			creationContext.AddUnresolvedReference(this, attributeName, creationContext.ProcessBlock, "attribute", typeof (IAttribute));

			this._state = creationContext.State;

			String accessText = xmlElement.GetProperty("access");
			creationContext.Check(((Object) accessText != null), "access is a required property in element field : " + xmlElement);
			this._access = FieldAccessHelper.fromText(accessText);
		}

        public virtual void ReadWebData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			this._name = xmlElement.GetProperty("name");
			this._description = xmlElement.GetProperty("description");
			this._index = creationContext.Index;

			log.Debug("paring web information for field " + _name);

			creationContext.DelegatingObject = this;
			XmlElement formatterElement = xmlElement.GetChildElement("htmlformatter");
			if (formatterElement != null)
			{
				this._htmlFormatterDelegation = new DelegationImpl();
				this._htmlFormatterDelegation.ReadProcessData(formatterElement, creationContext);
			}
			creationContext.DelegatingObject = null;
		}

        public virtual void Validate(ValidationContext validationContext)
		{
			validationContext.Check((_state != null), "state is a required property in a field");
			validationContext.Check((_attribute != null), "attribute is a required property in a field");
			validationContext.Check((_access != 0), "access is a required property in a field");
		}

        public virtual IHtmlFormatter GetHtmlFormatter()
		{
			if ((_htmlFormatter == null) && (_htmlFormatterDelegation != null))
			{
				_htmlFormatter = (IHtmlFormatter) _htmlFormatterDelegation.GetDelegate();
				log.Debug("created field.htmlFormatter delegate: " + _htmlFormatter);
			}
			return _htmlFormatter;
		}

	}
}