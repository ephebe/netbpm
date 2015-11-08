namespace NetBpm.Workflow.Organisation
{
	/// <summary> is a {@link User} or a {@link Group} to which a flow can be assigned.</summary>
	public interface IActor
	{
		/// <summary> is the id for this actor.  This is a text field because of flexibility.</summary>
		string Id { get; }

		/// <summary> the name for the actor.</summary>
		string Name { get; }
	}
}