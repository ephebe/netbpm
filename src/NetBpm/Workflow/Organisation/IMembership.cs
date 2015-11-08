using System;

namespace NetBpm.Workflow.Organisation
{
	/// <summary> represents the n-m relation between {@link User}s and {@link Group}s.</summary>
	public interface IMembership
	{
		/// <summary> the related {@link Group}.</summary>
		IGroup Group { get; }

		/// <summary> the related {@link User}.</summary>
		IUser User { get; }

		/// <summary> the role that the {@link User} performs for the {@link Group} <i>(optional)</i> </summary>
		String Role { get; }

		/// <summary> the type of Membership allows distinction between 'hierarchical'
		/// and 'supportive' Memberships.
		/// </summary>
		String Type { get; }
	}
}