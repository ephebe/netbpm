using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Execution.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class ProcessInstanceRepository
    {
        private static readonly ProcessInstanceRepository instance = new ProcessInstanceRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessInstanceRepository));

		/// <summary> gets the singleton instance.</summary>
		public static ProcessInstanceRepository Instance
		{
			get { return instance; }
		}

        private ProcessInstanceRepository()
		{
		}

        public void Save(ProcessInstanceImpl processInstance,DbSession dbSession) 
        {
            dbSession.Save(processInstance);
        }
    }
}
