using System;
using System.Collections;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ForkImpl : NodeImpl, IFork
	{
		[NonSerialized()] private DelegationImpl _forkDelegation = null;

        public virtual DelegationImpl ForkDelegation
		{
			get { return _forkDelegation; }
			set { this._forkDelegation = value; }
		}

		public ForkImpl()
		{
		}

		public ForkImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual DelegationImpl CreateForkDelegation()
		{
			_forkDelegation = new DelegationImpl(_processDefinition);
			return _forkDelegation;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);

			if ((Object) xmlElement.GetAttribute("handler") != null)
			{
				creationContext.DelegatingObject = this;
				this._forkDelegation = new DelegationImpl();
				this._forkDelegation.ReadProcessData(xmlElement, creationContext);
				creationContext.DelegatingObject = null;
			}
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);

			// verify that all transitions leaving a fork have a name
			// this is required because this name is assigned to the flow and
			// it serves as an id for the ForkHandler's
			IEnumerator iter = _leavingTransitions.GetEnumerator();
			while (iter.MoveNext())
			{
				TransitionImpl transition = (TransitionImpl) iter.Current;
				validationContext.Check(((Object) transition.Name != null), "one of the transitions leaving the fork does not have a name");
			}
		}
	}
}