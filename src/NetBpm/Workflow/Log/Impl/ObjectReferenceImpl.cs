using System;
using log4net;
using NetBpm.Util.DB;

namespace NetBpm.Workflow.Log.Impl
{
	public class ObjectReferenceImpl : LogDetailImpl, IObjectReference
	{
		private Int64 _referenceId;
		private String _className = null;
		private Object _object = null;
		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (ObjectReferenceImpl));

		public virtual Int64 ReferenceId
		{
			get { return this._referenceId; }
			set { this._referenceId = value; }
		}

        public virtual String ClassName
		{
			get { return this._className; }
			set { this._className = value; }
		}

		public ObjectReferenceImpl()
		{
		}

		public ObjectReferenceImpl(PersistentObject persistentObject)
		{
			this._referenceId = persistentObject.Id;
			this._className = persistentObject.GetType().FullName;
		}

		public override void Resolve(DbSession dbSession)
		{
			try
			{
				log.Debug("resolving object reference : " + _referenceId + " : " + _className);
				Type clazz = Type.GetType(_className);
				_object = dbSession.Load(clazz, _referenceId);
			}
			catch (System.Exception e)
			{
				log.Error("error resolving object reference :", e);
			}
		}

        public virtual Object GetObject()
		{
			return _object;
		}

	}
}