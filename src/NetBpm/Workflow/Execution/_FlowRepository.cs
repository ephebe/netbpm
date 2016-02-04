using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Execution.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class FlowRepository
    {
        private static readonly FlowRepository instance = new FlowRepository();
		private static readonly ILog log = LogManager.GetLogger(typeof (FlowRepository));

		/// <summary> gets the singleton instance.</summary>
		public static FlowRepository Instance
		{
			get { return instance; }
		}

        private FlowRepository()
		{
		}

        private const String queryFindConcurrentFlows = "select cf " +
            "from cf in class NetBpm.Workflow.Execution.Impl.FlowImpl," +
            "     f in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
            "where f.id = ? " + "  and cf.Parent = f.Parent " +
            "  and cf.EndNullable is null " + "  and cf.id <> f.id ";

        public IList GetOtherActiveConcurrentFlows(long flowId,DbSession dbSession)
        {
            return dbSession.Find(queryFindConcurrentFlows, flowId, DbType.LONG);
        }



        public FlowImpl GetFlow(long flowId,DbSession dbSession)
        {
            FlowImpl flow = (FlowImpl)dbSession.Load(typeof(FlowImpl), flowId);

            return flow;
        }
    }
}
