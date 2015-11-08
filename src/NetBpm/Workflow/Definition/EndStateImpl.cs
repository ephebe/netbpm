using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class EndStateImpl : StateImpl, IEndState
	{
		public EndStateImpl()
		{
		}

		public EndStateImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
		}

		protected internal override void ValidateLeavingTransitions(ValidationContext validationContext)
		{
			// overwriting the test of the node that requires that a node has to have
			// at least one leaving transition
		}
	}
}