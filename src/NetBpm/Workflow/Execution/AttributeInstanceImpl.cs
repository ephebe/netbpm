using System;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Execution.Impl
{
	public class AttributeInstanceImpl : PersistentObject, IAttributeInstance
	{
		private String _valueText = null;
		private IAttribute _attribute = null;
		private IFlow _scope = null;
		private bool _valueInitialized = false;
		private Object _attributeValue = null;
		private ISerializer _serializer = null;

		private static readonly ILog log = LogManager.GetLogger(typeof (AttributeInstanceImpl));

        public virtual String ValueText
		{
			get { return this._valueText; }
			set { this._valueText = value; }
		}

        public virtual IAttribute Attribute
		{
			get { return this._attribute; }
			set { this._attribute = value; }
		}

        public virtual IFlow Scope
		{
			get { return this._scope; }
			set { this._scope = value; }
		}

		public AttributeInstanceImpl()
		{
		}

		public AttributeInstanceImpl(AttributeImpl attribute, FlowImpl scope)
		{
			this._valueText = attribute.InitialValue;
			this._attribute = attribute;
			this._scope = scope;
		}

        public virtual Object GetValue()
		{
			log.Debug("getting value of attribute instance : " + _valueText + " : " + _attributeValue);
			Object initializedValue = null;
			if (_valueInitialized)
			{
				initializedValue = this._attributeValue;
			}
			else
			{
				if ((Object) _valueText != null)
				{
					InitSerializer();
					initializedValue = _serializer.Deserialize(_valueText);
					log.Debug("value set to : " + initializedValue);
				}
				else
				{
					initializedValue = null;
				}
				_valueInitialized = true;
				this._attributeValue = initializedValue;
			}
			return _attributeValue;
		}

        public virtual void SetValue(Object valueObject)
		{
			this._attributeValue = valueObject;
			this._valueInitialized = true;

			if (valueObject != null)
			{
				InitSerializer();
				ValueText = _serializer.Serialize(valueObject);
			}
			else
			{
				ValueText = null;
			}

			log.Debug("attribute " + Attribute.Name + " was set to " + valueObject + " (" + _valueText + ")");
		}

		public override String ToString()
		{
			return "attributeInstance[" + _id + "|" + _attribute.Name + "|" + _scope.Id + "]";
		}

		private void InitSerializer()
		{
			if (_serializer == null)
			{
				AttributeImpl attributeImpl = (AttributeImpl) this._attribute;
				_serializer = (ISerializer) attributeImpl.SerializerDelegation.GetDelegate();
			}
		}
	}
}