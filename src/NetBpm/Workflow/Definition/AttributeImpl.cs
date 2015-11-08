using System;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class AttributeImpl : DefinitionObjectImpl, IAttribute
	{
		private IProcessBlock _scope = null;
		private String _initialValue = null;
		private DelegationImpl _serializerDelegation = null;

		public virtual IProcessBlock Scope
		{
			set { _scope = value; }
			get { return _scope; }
		}

        public virtual String InitialValue
		{
			set { _initialValue = value; }
			get { return _initialValue; }
		}

        public virtual IDelegation SerializerDelegation
		{
			set { _serializerDelegation = (DelegationImpl) value; }
			get { return _serializerDelegation; }
		}

		public AttributeImpl()
		{
		}

		public AttributeImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual DelegationImpl CreateSerializerDelegation()
		{
			_serializerDelegation = new DelegationImpl(_processDefinition);
			return _serializerDelegation;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);
			this._scope = creationContext.ProcessBlock;
			this._initialValue = xmlElement.GetProperty("initial-value");

			creationContext.DelegatingObject = this;
			this._serializerDelegation = new DelegationImpl();
			this._serializerDelegation.ReadProcessData(xmlElement, creationContext);
			creationContext.DelegatingObject = null;
			creationContext.AddReferencableObject(_name, (ProcessBlockImpl) this._scope, typeof (IAttribute), this);
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
		}
	}
}