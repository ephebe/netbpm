using System;
using log4net;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
    /// <summary>
    /// Åª¨úState¤¤ªºassignment¡Arole
    /// </summary>
	public class ActivityStateImpl : StateImpl, IActivityState
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (ActivityStateImpl));
		private String _actorRoleName = null;
		[NonSerialized()] private DelegationImpl _assignmentDelegation = null;

        public virtual String ActorRoleName
		{
			set { _actorRoleName = value; }
			get { return _actorRoleName; }
		}

        public virtual DelegationImpl AssignmentDelegation
		{
			get { return _assignmentDelegation; }
			set { this._assignmentDelegation = value; }
		}

		public ActivityStateImpl()
		{
		}

		public ActivityStateImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
		}

        public virtual DelegationImpl CreateAssignmentDelegation()
		{
			_assignmentDelegation = new DelegationImpl(_processDefinition);
			return _assignmentDelegation;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);

			XmlElement assignmentElement = xmlElement.GetChildElement("assignment");
			if (assignmentElement != null)
			{
				creationContext.DelegatingObject = this;
				this._assignmentDelegation = new DelegationImpl();
				this._assignmentDelegation.ReadProcessData(assignmentElement, creationContext);
				creationContext.DelegatingObject = null;
			}
			this._actorRoleName = xmlElement.GetProperty("role");
		}

		public override void ReadWebData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadWebData(xmlElement, creationContext);
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
			if (!(this is StartStateImpl))
			{
				validationContext.Check(((_assignmentDelegation != null) || ((Object) _actorRoleName != null)), "ActivityState '" + Name + "' does not have an assignment or a role.  It should have at least one of the two.");
			}
		}
	}
}