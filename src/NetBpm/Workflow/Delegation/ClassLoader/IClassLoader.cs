using System;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Delegation.ClassLoader
{
	public interface IClassLoader
	{
		Object CreateObject(DelegationImpl delegationImpl);
	}
}
