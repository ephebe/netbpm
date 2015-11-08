using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Execution
{
	/// <summary> is a RuntimeException that is thrown when an actor performs an activity and no value is supplied 
	/// for one of the required fields.
	/// </summary>
	public class RequiredFieldException : ExecutionException
	{
		private IField _field = null;

		public IField Field
		{
			get { return _field; }
		}

		public RequiredFieldException(IField field) : base("field '" + field.Attribute.Name + "' was required and not submitted in the perform of state '" + field.State.Name + "'")
		{
			this._field = field;
		}
	}
}