using System;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ProcessStateImpl : StateImpl, IProcessState
	{
		private IProcessDefinition _subProcess = null;
		[NonSerialized()] private DelegationImpl _processInvokerDelegation = null;
		private String _actorExpression = null;

		private const String queryFindProcessDefinitionByName = "select pd " +
			"from pd in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"where pd.Name = ? " +
			"  and pd.Version = ( " +
			"    select max(pd2.Version) " +
			"    from pd2 in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"    where pd2.Name = pd.Name )";

        public virtual IProcessDefinition SubProcess
		{
			set { _subProcess = value; }
			get { return _subProcess; }
		}

        public virtual string ActorExpression
		{
			set { _actorExpression = value; }
			get { return _actorExpression; }
		}

        public virtual DelegationImpl ProcessInvokerDelegation
		{
			get { return _processInvokerDelegation; }
			set { this._processInvokerDelegation = value; }
		}

		public ProcessStateImpl()
		{
		}

		public ProcessStateImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual DelegationImpl CreateProcessInvocationDelegation()
		{
			_processInvokerDelegation = new DelegationImpl(_processDefinition);
			return _processInvokerDelegation;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);

			// get the process definition for that name
			String subProcessDefinitionName = xmlElement.GetProperty("process");
			creationContext.Check(((Object) subProcessDefinitionName != null), "process is missing in the process state : " + subProcessDefinitionName);
			DbSession dbSession = creationContext.DbSession;
			dbSession.SaveOrUpdate(this._processDefinition);
			try
			{
				this._subProcess = (ProcessDefinitionImpl) dbSession.FindOne(queryFindProcessDefinitionByName, subProcessDefinitionName, DbType.STRING);
			}
			catch (SystemException e)
			{
				creationContext.AddError("process '" + subProcessDefinitionName + "' was not deployed while it is referenced in a process-state. Exception: " + e.Message);
			}

			// parse the processInvokerDelegation
			creationContext.DelegatingObject = this;
			this._processInvokerDelegation = new DelegationImpl();
			XmlElement invocationElement = xmlElement.GetChildElement("process-invocation");
			creationContext.Check((invocationElement != null), "process-invocation is missing in the process-state : " + xmlElement);
			this._processInvokerDelegation.ReadProcessData(invocationElement, creationContext);

			creationContext.DelegatingObject = null;

			// parse the actorExpression
			this._actorExpression = xmlElement.GetProperty("actor-expression");
			creationContext.Check(((Object) _actorExpression != null), "actor-expression is missing in the process-state : " + xmlElement);
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
			validationContext.Check((_processInvokerDelegation != null), "process state does not have a process invoker delegation");
			validationContext.Check((_subProcess != null), "process state does not have a sub-process-definition");
		}
	}
}