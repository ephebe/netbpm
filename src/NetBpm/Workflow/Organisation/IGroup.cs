using System.Collections;

namespace NetBpm.Workflow.Organisation
{
	/// <summary> is any type of collection of users.
	/// Organisational entities such as a business unit, a department, a team, ...
	/// or a goup could be created for e.g. : all the people with red hair.
	/// Groups can be ordered hierarchically.  This is mostly the case in 
	/// groups that represent the organisational hierarchy.
	/// 
	/// <p>Since it's not possible to capture all group-information
	/// that is used in all organisations in this interface, a basic default
	/// set of properties is provided and it is made easy to extend them
	/// by customizing this organisation component.</p>
	/// 
	/// <p><img src="organisationmodel.gif"></img></p>
	/// </summary>
	public interface IGroup : IActor
	{
		/// <summary> gets all the {@link Membership}s of this Group.</summary>
		/// <returns> a Collection of {@link Membership}s.
		/// </returns>
		ICollection Memberships { get; }

		/// <summary> gets the parent-Group of this group.</summary>
		IGroup Parent { get; }

		/// <summary> gets the child-Groups of this group.</summary>
		/// <returns> a Collection of Groups.
		/// </returns>
		ICollection Children { get; }
	}
}