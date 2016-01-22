using log4net;
using NetBpm.Util.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class FieldRepository
    {
        private static readonly FieldRepository instance = new FieldRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(FieldRepository));

		/// <summary> gets the singleton instance.</summary>
		public static FieldRepository Instance
		{
			get { return instance; }
		}

        private FieldRepository()
		{
		}

        private const String queryFieldsByState = "select f from f in class NetBpm.Workflow.Definition.Impl.FieldImpl " +
            "where f.State.Id = ? " +
            "order by f.Index";

        public IList FindFieldsByState(long stateId,DbSession dbSession) 
        {
           return dbSession.Find(queryFieldsByState, stateId, DbType.LONG);
        }
    }
}
