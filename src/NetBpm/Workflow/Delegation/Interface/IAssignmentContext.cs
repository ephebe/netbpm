using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows the Assigner-implementator to interact with and get information from
	/// the process execution engine.
	/// </summary>
	public interface IAssignmentContext : IHandlerContext
	{
		/// <summary> since Assigner-implementations tend to use the organisation-component 
		/// this is a convenient way to get it.
		/// </summary>
		IOrganisationService GetOrganisationComponent();

		/// <summary> gets the {@link ActivityState} for which an {@link Actor} has to be selected. </summary>
		new INode GetNode();

		/// <summary> gets the {@link Actor} that performed the previous activity-state.</summary>
		IActor GetPreviousActor();
	}
}