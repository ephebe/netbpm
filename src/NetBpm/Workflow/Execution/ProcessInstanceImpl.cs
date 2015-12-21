using System;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Execution.Impl
{
	public partial class ProcessInstanceImpl : PersistentObject, IProcessInstance
	{
	    private DateTime? _start = null;
	    private DateTime? _end = null;
		private bool active = true;
		private String _initiatorActorId = null;
		private IProcessDefinition _processDefinition = null;
		private IFlow _rootFlow = null;
		private IFlow _superProcessFlow = null;
		private static readonly OrganisationUtil organisationUtil = OrganisationUtil.Instance;

        public virtual bool StartHasValue
		{
			get { return this._start.HasValue; }
		}

        public virtual bool EndHasValue
		{
			get { return this._end.HasValue; }
		}

        public virtual DateTime? EndNullable
		{
			get { return this._end; }
			set { this._end = value; }
		}

        public virtual DateTime? StartNullable
		{
			get { return this._start; }
			set { this._start = value; }
		}

        public virtual DateTime End
		{
			get { return this._end.Value; }
			set { this._end = value; }
		}

        public virtual DateTime Start
		{
			get { return this._start.Value; }
			set { this._start = value; }
		}

        public virtual String InitiatorActorId
		{
			get { return _initiatorActorId; }
			set { this._initiatorActorId = value; }
		}

        public virtual IProcessDefinition ProcessDefinition
		{
			get { return this._processDefinition; }
			set { this._processDefinition = value; }
		}

        public virtual IFlow RootFlow
		{
			get { return this._rootFlow; }
			set { this._rootFlow = value; }
		}

        public virtual IFlow SuperProcessFlow
		{
			get { return this._superProcessFlow; }
			set { this._superProcessFlow = value; }
		}

        public virtual bool Active
		{
			get { return active; }
		}

		public ProcessInstanceImpl()
		{
		}

		public ProcessInstanceImpl(String actorId, ProcessDefinitionImpl processDefinition)
		{
			this._start = DateTime.Now;
			this._initiatorActorId = actorId;
			this._processDefinition = processDefinition;
			this._rootFlow = new FlowImpl(actorId, this);
		}

        public virtual IActor GetInitiator()
		{
			return organisationUtil.GetActor(_initiatorActorId);
		}

		public override String ToString()
		{
			return "processInstance[" + _id + "|" + _processDefinition.Name + "]";
		}
	}
}