using System;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution.Impl;

namespace NetBpm.Workflow.Log.Impl
{
	public class AttributeUpdateImpl : LogDetailImpl, IAttributeUpdate
	{
		private IAttribute _attribute = null;
		private String _valueText = null;

		public virtual IAttribute Attribute
		{
			get { return this._attribute; }
			set { this._attribute = value; }
		}

        public virtual String ValueText
		{
			get { return this._valueText; }
			set { this._valueText = value; }
		}

		public AttributeUpdateImpl()
		{
		}

		public AttributeUpdateImpl(AttributeInstanceImpl attributeInstance)
		{
			this._attribute = attributeInstance.Attribute;
			this._valueText = attributeInstance.ValueText;
		}

        public virtual Object GetValue()
		{
			// until it is possible to access a default html-formatter
			// for an attribute, it is not possible to give the real 
			// object here, sorry. 
			return this._valueText;
		}
	}
}