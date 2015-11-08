using System;
using Iesi.Collections;

namespace NetBpm.Workflow.Organisation
{
	/// <summary> represents a human user or an IT-system.
	/// <p>In most methods of the ExecutionSession-bean, that bean needs to 
	/// to know which User is performing that operation.  The userName is 
	/// used in the ExecutionSession-bean to identify the User-object.</p>
	/// 
	/// <p>Since it's not possible to capture all user-information
	/// that is used in all organisations in this interface, a basic default
	/// set of properties is provided and it is made easy to extend them
	/// by customizing this organisation component.</p>
	/// </summary>
	public interface IUser : IActor
	{
		String FirstName { get; }

		String LastName { get; }

		String Email { get; }

		ISet Memberships { get; }
	}
}