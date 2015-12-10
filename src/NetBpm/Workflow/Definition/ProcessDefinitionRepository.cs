using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ProcessDefinitionRepository
	{
		private static readonly ProcessDefinitionRepository instance = new ProcessDefinitionRepository();
		private static readonly ILog log = LogManager.GetLogger(typeof (ProcessDefinitionRepository));

		/// <summary> gets the singleton instance.</summary>
		public static ProcessDefinitionRepository Instance
		{
			get { return instance; }
		}

		private ProcessDefinitionRepository()
		{
		}

		private const String queryFindProcessDefinitions = "select pd " + "from pd in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"where pd.Version = ( " + "    select max(pd2.Version) " +
			"    from pd2 in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"    where pd2.Name = pd.Name )";

		public IList GetProcessDefinitions(Relations relations, DbSession dbSession)
		{
			IList processDefinitions = null;
			processDefinitions = dbSession.Find(queryFindProcessDefinitions);
			if (relations != null)
			{
				relations.Resolve(processDefinitions);
			}
			return processDefinitions;
		}

		private const String queryFindProcessDefinitionByName = "select pd " +
			"from pd in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"where pd.Name = ? " + "  and pd.Version = ( " + "    select max(pd2.Version) " +
			"    from pd2 in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl " +
			"    where pd2.Name = pd.Name )";

		public IProcessDefinition GetProcessDefinition(String processDefinitionName, Relations relations, DbSession dbSession)
		{
			ProcessDefinitionImpl processDefinition = null;
			processDefinition = (ProcessDefinitionImpl) dbSession.FindOne(queryFindProcessDefinitionByName, processDefinitionName, DbType.STRING);
			if (relations != null)
			{
				relations.Resolve(processDefinition);
			}
			return processDefinition;
		}

		public IProcessDefinition GetProcessDefinition(Int64 processDefinitionId, Relations relations, DbSession dbSession)
		{
			ProcessDefinitionImpl processDefinition = null;
			processDefinition = (ProcessDefinitionImpl) dbSession.Load(typeof (ProcessDefinitionImpl), processDefinitionId);
			if (relations != null)
			{
				relations.Resolve(processDefinition);
			}
			return processDefinition;
		}

		private const String queryFindAllProcessDefinitions = "from pd in class NetBpm.Workflow.Definition.Impl.ProcessDefinitionImpl";

		public IList GetAllProcessDefinitions(Relations relations, DbSession dbSession)
		{
			IList processDefinitions = null;
			log.Debug("getting all process definitions...");
			processDefinitions = dbSession.Find(queryFindAllProcessDefinitions);
			if (relations != null)
			{
				relations.Resolve(processDefinitions);
			}
			return processDefinitions;
		}

        public void Save(ProcessDefinitionImpl processDefinition,DbSession dbSession) 
        {
            try
            {
                dbSession.SaveOrUpdate(processDefinition);
                dbSession.Flush();
            }
            catch (DbException e)
            {
                throw new NpdlException("couldn't deploy process archive due to a database exception : " + e.Message, e);
            }
        }
	}
}