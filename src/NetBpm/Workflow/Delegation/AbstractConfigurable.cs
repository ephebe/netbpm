using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	public class AbstractConfigurable : IConfigurable
	{
		private IDictionary _configuration = null;

		public IDictionary GetConfiguration()
		{
			return this._configuration;
		}

		public void SetConfiguration(IDictionary configuration)
		{
			this._configuration = configuration;
		}
	}
}