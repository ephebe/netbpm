using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	public interface IConfigurable
	{
		void SetConfiguration(IDictionary configuration);
	}
}