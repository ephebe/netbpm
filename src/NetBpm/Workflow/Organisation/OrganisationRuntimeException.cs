using System;

namespace NetBpm.Workflow.Organisation
{
	/// <summary> is a RuntimeException that is thrown to signal any kind of exception while retrieving organisational information.</summary>
	public class OrganisationRuntimeException : SystemException
	{
		public OrganisationRuntimeException()
		{
		}

		public OrganisationRuntimeException(String msg) : base(msg)
		{
		}
	}
}