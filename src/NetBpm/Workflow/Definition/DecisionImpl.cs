using System;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class DecisionImpl : NodeImpl, IDecision
	{
		[NonSerialized()] private DelegationImpl _decisionDelegation = null;

        public virtual DelegationImpl DecisionDelegation
		{
			get { return this._decisionDelegation; }

			set { this._decisionDelegation = value; }

		}

		public DecisionImpl()
		{
		}

		public DecisionImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual DelegationImpl CreateDecisionDelegation()
		{
			_decisionDelegation = new DelegationImpl(_processDefinition);
			return _decisionDelegation;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);
			creationContext.DelegatingObject = this;
			this._decisionDelegation = new DelegationImpl();
			this._decisionDelegation.ReadProcessData(xmlElement, creationContext);
			creationContext.DelegatingObject = null;
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
			validationContext.Check((_decisionDelegation != null), "no decision delegation specified");
		}
	}
}