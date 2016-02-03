using System;
using System.Collections;

namespace NetBpm.Workflow.Definition
{
	public interface IState : INode
	{
		Int32[] ImageCoordinates { get; }

        void CheckAccess(IDictionary attributeValues);
	}
}