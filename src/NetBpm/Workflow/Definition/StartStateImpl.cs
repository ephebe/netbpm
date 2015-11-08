using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class StartStateImpl : ActivityStateImpl, IStartState
	{
		public StartStateImpl()
		{
		}

		public StartStateImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			creationContext.DefinitionObject = this;
			base.ReadProcessData(xmlElement, creationContext);
			creationContext.DefinitionObject = null;
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
		}
	}
}