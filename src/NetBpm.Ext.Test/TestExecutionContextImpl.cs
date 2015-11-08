using System;
using System.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Scheduler;

namespace NetBpm.Boo.Test
{
	/// <summary>
	/// Zusammenfassung für TestExecutionContextImpl.
	/// </summary>
	public class TestExecutionContextImpl : IActionContext
	{
		private IDictionary _configuration = null;
		private IDictionary attributes = null;

		public TestExecutionContextImpl() 
		{
			this.attributes = new Hashtable();
			this._configuration = new Hashtable();
		}

		public virtual IDictionary GetConfiguration()
		{
			return this._configuration;
		}

		public void SetConfiguration(IDictionary configuration)
		{
			this._configuration = configuration;
		}

		public IProcessDefinition GetProcessDefinition()
		{
			throw new NotImplementedException();
		}

		public IProcessInstance GetProcessInstance()
		{
			throw new NotImplementedException();
		}

		public IFlow GetFlow()
		{
			throw new NotImplementedException();
		}

		public void SetAttribute(String attributeName, Object attributeValue)
		{
			attributes[attributeName]=attributeValue;
		}

		public INode GetNode()
		{
			throw new NotImplementedException();
		}

		public Object GetAttribute(String name)
		{
			return attributes[name];
		}

		public void AddLog(String msg)
		{
			throw new NotImplementedException();
		}

		public void Schedule(Job job)
		{
			throw new NotImplementedException();
		}

		public void Schedule(Job job, String reference)
		{
			throw new NotImplementedException();
		}

	}
}
