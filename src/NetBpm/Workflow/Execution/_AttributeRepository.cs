using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Execution.Impl;
using NHibernate.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class AttributeRepository
    {
        private static readonly AttributeRepository instance = new AttributeRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(AttributeRepository));

		/// <summary> gets the singleton instance.</summary>
		public static AttributeRepository Instance
		{
			get { return instance; }
		}

        private AttributeRepository()
		{
		}

        private const String queryFindAttributeInstanceByName = "select ai " +
            "from ai in class NetBpm.Workflow.Execution.Impl.AttributeInstanceImpl, " +
            "     f in class NetBpm.Workflow.Execution.Impl.FlowImpl " +
            "where ai.Scope = f.id " + "  and ai.Attribute.Name = ? " +
            "  and f.id = ? ";

        public IList FindAttributeInstanceByName(string attributeName,long scopeId,DbSession dbSession) 
        {
            Object[] values = new Object[] { attributeName, scopeId };
            IType[] types = new IType[] { DbType.STRING, DbType.LONG };

            return dbSession.Find(queryFindAttributeInstanceByName, values, types);
        }
    }
}
