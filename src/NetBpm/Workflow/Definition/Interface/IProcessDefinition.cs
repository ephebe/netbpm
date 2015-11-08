using System;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Definition
{
	public interface IProcessDefinition : IProcessBlock
	{
		String ResponsibleUserName { get; }
		Int32 Version { get; }
		IStartState StartState { get; }
		IEndState EndState { get; }
		byte[] Image { get; }
		String ImageMimeType { get; }
		Int32 ImageHeight { get; }
		Int32 ImageWidth { get; }

		IUser GetResponsible();
	}
}