using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class TransitionImpl : DefinitionObjectImpl, ITransition
	{
		private INode _from = null;
		private INode _to = null;

		public TransitionImpl()
		{
		}

		public TransitionImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			DefinitionObjectImpl parent = creationContext.DefinitionObject;
			creationContext.DefinitionObject = this;
			base.ReadProcessData(xmlElement, creationContext);
			creationContext.DefinitionObject = parent;
			this._from = creationContext.Node;

			creationContext.AddUnresolvedReference(this, xmlElement.GetProperty("to"), creationContext.TransitionDestinationScope, "to", typeof (INode));
		}

        public virtual INode From
		{
			set { _from = value; }
			get { return _from; }
		}

        public virtual INode To
		{
			set { _to = value; }
			get { return _to; }
		}
	}
}