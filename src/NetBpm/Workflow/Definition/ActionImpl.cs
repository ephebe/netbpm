using System;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NetBpm.Workflow.Delegation.Impl;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ActionImpl : PersistentObject, IAction
	{
		[NonSerialized()] private DelegationImpl _actionDelegation = null;
		private static readonly ILog log = LogManager.GetLogger(typeof (ActionImpl));

		private Int64 _definitionObjectId;
		private EventType _eventType = 0;

        public virtual Int64 DefinitionObjectId
		{
			get { return this._definitionObjectId; }
			set { this._definitionObjectId = value; }
		}

        public virtual DelegationImpl ActionDelegation
		{
			get { return this._actionDelegation; }
			set { this._actionDelegation = value; }
		}

        public virtual EventType EventType
		{
			set { _eventType = value; }
			get { return _eventType; }
		}

		public ActionImpl()
		{
		}

        public virtual void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			DefinitionObjectImpl definitionObject = creationContext.DefinitionObject;

			// first make sure the definitionObject has got an id
			DbSession dbSession = creationContext.DbSession;
			dbSession.SaveOrUpdate(definitionObject);

			// store the reference link to the definitionObject 
			this._definitionObjectId = definitionObject.Id;

			log.Debug("adding action : ");
			log.Debug("  definitionObjectId: " + _definitionObjectId);
			log.Debug("  definitionObject: " + definitionObject);

			this._eventType = EventTypeHelper.fromText(xmlElement.GetAttribute("event"));

			log.Debug("action on eventType '" + _eventType + "' and definitionObject " + creationContext.DefinitionObject);

			// reading the action delegation
			creationContext.DelegatingObject = this;
			this._actionDelegation = new DelegationImpl();
			this._actionDelegation.ReadProcessData(xmlElement, creationContext);
			creationContext.DelegatingObject = null;

			dbSession.SaveOrUpdate(this);
		}

        public virtual void Validate(ValidationContext validationContext)
		{
		}
	}
}