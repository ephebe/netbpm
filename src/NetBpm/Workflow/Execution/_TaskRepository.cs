using log4net;
using NetBpm.Util.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class TaskRepository
    {
        private static readonly TaskRepository instance = new TaskRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(TaskRepository));

		/// <summary> gets the singleton instance.</summary>
		public static TaskRepository Instance
		{
			get { return instance; }
		}

        private TaskRepository()
		{
		}

        private const String queryFindTasks = "select flow " +
            "from flow in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
            "where flow.ActorId = ?";

        public IList FindTasks(string actorId,DbSession dbSession) 
        {
            return dbSession.Find(queryFindTasks, actorId, DbType.STRING);
        }

    }
}
