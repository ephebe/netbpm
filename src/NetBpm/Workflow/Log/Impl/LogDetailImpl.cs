using NetBpm.Util.DB;

namespace NetBpm.Workflow.Log.Impl
{
	public abstract class LogDetailImpl : PersistentObject, ILogDetail
	{
		private ILog _log = null;

        public virtual ILog Log
		{
			get { return this._log; }
			set { this._log = value; }
		}

		public LogDetailImpl()
		{
		}

		public LogDetailImpl(LogImpl log)
		{
			this._log = log;
		}

		/// <summary> allows sub-classes to do some resolving using the database on the server side.</summary>
		public virtual void Resolve(DbSession dbSession)
		{
		}
	}
}