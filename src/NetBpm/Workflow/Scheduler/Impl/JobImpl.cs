using System;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution;

namespace NetBpm.Workflow.Scheduler.Impl
{
	public class JobImpl
	{
		private Int64 _id;
		private IProcessDefinition _processDefinition = null;
		private IFlow _context = null;
		private DateTime _date = DateTime.MinValue;
//		private String _taskClassName = null;
//		private String _configuration = null;
		private String _userId = null;
		private String _pwd = null;
		private String _reference = null;
		private DelegationImpl _actionDelegation = null;

		public virtual Int64 Id
		{
			get { return _id; }
			set { this._id = value; }
		}

        public virtual IProcessDefinition ProcessDefinition
		{
			get { return _processDefinition; }
			set { this._processDefinition = value; }
		}

        public virtual IFlow Context
		{
			get { return this._context; }
			set { this._context = value; }
		}

        public virtual DateTime Date
		{
			get { return _date; }
			set { this._date = value; }
		}

        public virtual String UserId
		{
			get { return _userId; }
			set { this._userId = value; }
		}

        public virtual String Pwd
		{
			get { return _pwd; }
			set { this._pwd = value; }
		}

        public virtual String Reference
		{
			get { return _reference; }
			set { this._reference = value; }
		}

        public virtual DelegationImpl ActionDelegation
		{
			get { return this._actionDelegation; }
			set { this._actionDelegation = value; }
		}

		public JobImpl()
		{
		}

		public JobImpl(Job job, String reference)
		{
			_processDefinition = job.ProcessDefinition;
			_context = job.Context;
			_date = job.Date;
			_userId = job.UserId;
			_pwd = job.Pwd;

			this._reference = reference;
			_actionDelegation = new DelegationImpl();
			_actionDelegation.ProcessDefinition = job.ProcessDefinition;
			_actionDelegation.ClassName = job.TaskClassName;
			_actionDelegation.Configuration = job.Configuration;
		}

		public override String ToString()
		{
			return "activation[" + _id + "|" + _actionDelegation.ClassName + "|" + _date.ToString("r") + "]";
		}
	}
}