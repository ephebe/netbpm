using System;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;

namespace NetBpm.Workflow.Scheduler
{
	/// <summary> is all information needed to execute one task at a given time.</summary>
	public class Job
	{
		public const long SECOND = 1000;
		public static readonly long MINUTE = 60*SECOND;
		public static readonly long HOUR = 60*MINUTE;
		public static readonly long DAY = 24*HOUR;

		private IProcessDefinition _processDefinition = null;
		private IFlow _context = null;
		private DateTime _date;
		private String _taskClassName = null;
		private String _configuration = null;
		private String _userId = null;
		private String _pwd = null;

		public IProcessDefinition ProcessDefinition
		{
			get { return _processDefinition; }
			set { this._processDefinition = value; }
		}

		public IFlow Context
		{
			get { return this._context; }
			set { this._context = value; }
		}

		public DateTime Date
		{
			get { return _date; }
			set { this._date = value; }
		}

		public long Delay
		{
			set { this._date = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + value); }

		}

		public String TaskClassName
		{
			get { return _taskClassName; }
			set { this._taskClassName = value; }
		}

		public String Configuration
		{
			get { return _configuration; }
			set { this._configuration = value; }
		}

		public String UserId
		{
			get { return _userId; }
		}

		public String Pwd
		{
			get { return _pwd; }
		}

		protected internal Job()
		{
		}

		public Job(String taskClassName)
		{
			if ((Object) taskClassName == null)
				throw new SystemException("couldn't create a Job with a null-value for taskClassName");
			this._taskClassName = taskClassName;
		}

		public Job(IProcessDefinition processDefinition, String taskClassName)
		{
			if (processDefinition == null)
			{
				throw new SystemException("couldn't create a Job with a null-value for processDefinition");
			}

			if ((Object) taskClassName == null)
			{
				throw new SystemException("couldn't create a Job with a null-value for taskClassName");
			}
			this._taskClassName = taskClassName;
			this._processDefinition = processDefinition;
		}

		public Job(IFlow context, String taskClassName)
		{
			if (_processDefinition == null)
			{
				throw new SystemException("couldn't create a Job with a null-value for processDefinition");
			}
			if ((Object) taskClassName == null)
			{
				throw new SystemException("couldn't create a Job with a null-value for taskClassName");
			}

			this._taskClassName = taskClassName;
			this._context = context;
			this._processDefinition = context.ProcessInstance.ProcessDefinition;
		}

		/// <summary> this only authenticates calls to other beans made from the scheduled action.</summary>
		public void SetAuthentication(String userId, String pwd)
		{
			this._userId = userId;
			this._pwd = pwd;
		}
	}
}