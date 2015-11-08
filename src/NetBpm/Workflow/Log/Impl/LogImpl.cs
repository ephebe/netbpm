using System;
using System.Collections;
using Iesi.Collections;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Log.Impl
{
	public class LogImpl : PersistentObject, ILog, IComparable
	{
		private String _actorId = null;
		private IFlow _flow = null;
		private DateTime _date = DateTime.MinValue;
		private EventType _eventType = 0;
		private ISet _details = null;
		private static OrganisationUtil organisationUtil = OrganisationUtil.Instance;

        public virtual IFlow Flow
		{
			get { return this._flow; }
			set { this._flow = value; }
		}

        public virtual DateTime Date
		{
			get { return this._date; }
			set { this._date = value; }
		}

        public virtual EventType EventType
		{
			get { return this._eventType; }
			set { this._eventType = value; }
		}

        public virtual ISet Details
		{
			get { return this._details; }
			set { this._details = value; }
		}

        public virtual String ActorId
		{
			get { return this._actorId; }
			set { this._actorId = value; }
		}

		public LogImpl()
		{
		}

		public LogImpl(DateTime date, EventType eventType, IFlow flow)
		{
			this._date = date;
			this._eventType = eventType;
			this._flow = flow;
			this._details = new ListSet();
		}

		public LogImpl(String actorId, DateTime date, EventType eventType, IFlow flow)
		{
			this._actorId = actorId;
			this._date = date;
			this._eventType = eventType;
			this._flow = flow;
			this._details = new ListSet();
		}

        public virtual IActor GetActor()
		{
			return organisationUtil.GetActor(_actorId);
		}

        public virtual IList GetObjectReferences(String className)
		{
			IList objectReferences = new ArrayList();
			IEnumerator iter = this._details.GetEnumerator();
			while (iter.MoveNext())
			{
				ILogDetail logDetail = (ILogDetail) iter.Current;
				if (logDetail is ObjectReferenceImpl)
				{
					ObjectReferenceImpl objectReference = (ObjectReferenceImpl) logDetail;
					if (objectReference.ClassName.IndexOf(className) != - 1)
					{
						objectReferences.Add(objectReference);
					}
				}
			}
			return objectReferences;
		}


		public override String ToString()
		{
			return "log[" + _id + "|" + _eventType.ToString() + "|" + _date.ToString("r") + "]";
		}

        public virtual int CompareTo(Object otherEvent)
		{
			DateTime otherEventDate = ((LogImpl) otherEvent).Date;
			return (int) (_date.Ticks - otherEventDate.Ticks);
		}
	}
}