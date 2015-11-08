using System.Collections;
using Iesi.Collections;
using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class NodeImpl : DefinitionObjectImpl, INode
	{
		protected IProcessBlock _processBlock = null;
		protected ISet _arrivingTransitions = null;
		protected ISet _leavingTransitions = null;

        public virtual IProcessBlock ProcessBlock
		{
			set { _processBlock = value; }
			get { return _processBlock; }
		}

        public virtual ISet ArrivingTransitions
		{
			set { _arrivingTransitions = value; }
			get { return _arrivingTransitions; }
		}

        public virtual ISet LeavingTransitions
		{
			set { _leavingTransitions = value; }
			get { return _leavingTransitions; }
		}

		/// <summary> standard, the transition destination scope it the same scope as the node's scope.
		/// Only the JoinImpl will have a different opinion on this :-)
		/// </summary>
		protected internal virtual ProcessDefinitionBuildContext TransitionDestinationScope
		{
			set { value.TransitionDestinationScope = value.ProcessBlock; }

		}

		public NodeImpl()
		{
		}

		public NodeImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
			this._arrivingTransitions = new ListSet();
			this._leavingTransitions = new ListSet();
		}

        public virtual TransitionImpl CreateLeavingTransition()
		{
			TransitionImpl transition = new TransitionImpl(_processDefinition);
			transition.From = this;
			_leavingTransitions.Add(transition);
			return transition;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);

			this._arrivingTransitions = new ListSet();
			this._leavingTransitions = new ListSet();
			this._processBlock = creationContext.ProcessBlock;

			creationContext.Node = this;
			this.TransitionDestinationScope = creationContext;
			IEnumerator iter = xmlElement.GetChildElements("transition").GetEnumerator();
			while (iter.MoveNext())
			{
				XmlElement transitionElement = (XmlElement) iter.Current;
				TransitionImpl transition = new TransitionImpl();
				transition.ReadProcessData(transitionElement, creationContext);
				_leavingTransitions.Add(transition);
			}
			creationContext.TransitionDestinationScope = null;
			creationContext.Node = null;
			creationContext.AddReferencableObject(_name, (ProcessBlockImpl) this._processBlock, typeof (INode), this);
		}

		public override void Validate(ValidationContext validationContext)
		{
			validationContext.PushScope("in " + TypeName + " '" + _name + "'");
			base.Validate(validationContext);
			this.ValidateLeavingTransitions(validationContext);
			validationContext.PopScope();
		}

		/// <summary> standard, a node must have at least one leaving transition.
		/// Only the EndStateImpl will have a different opinion on this :-)
		/// </summary>
		protected internal virtual void ValidateLeavingTransitions(ValidationContext validationContext)
		{
			validationContext.Check((_leavingTransitions.Count > 0), "no transitions leaving this node");
		}
	}
}