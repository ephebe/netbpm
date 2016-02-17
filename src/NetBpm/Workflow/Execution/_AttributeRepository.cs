using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Execution.Impl;
using NHibernate.Mapping;
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

        public const String queryFindAttribute = "select a" +
            "from a in class NetBpm.Workflow.Definition.Impl.AttributeImpl" + 
            "where a.name = ? and a.Scope = ?";

        public IList FindAttributeByName(string attributeName, long processBlockId, DbSession dbSession)
        {
            throw new NotImplementedException();
        }

        public IList FindAttributeInstanceByName(string attributeName,long flowId,DbSession dbSession) 
        {
            Object[] values = new Object[] { attributeName, flowId };
            IType[] types = new IType[] { DbType.STRING, DbType.LONG };

            IList attributes = dbSession.Find(queryFindAttributeInstanceByName, values, types);
        
            return attributes;

        }

        public AttributeInstanceImpl FindAttributeInstanceInScope(String attributeName, FlowImpl scope,DbSession dbSession)
        {
            AttributeInstanceImpl attributeInstance = null;

            while (attributeInstance == null)
            {
                IList attributes = this.FindAttributeInstanceByName(attributeName, scope.Id, dbSession);
                IEnumerator iter = attributes.GetEnumerator();
                if (iter.MoveNext())
                {
                    attributeInstance = (AttributeInstanceImpl)iter.Current;
                    if (iter.MoveNext())
                    {
                        throw new NetBpm.Util.DB.DbException("duplicate value");
                    }
                }
                else
                {
                    if (!scope.IsRootFlow())
                    {
                        scope = (FlowImpl)scope.Parent;
                    }
                    else
                    {
                        log.Warn("couldn't find attribute-instance '" + attributeName + "' in scope of flow '" + scope + "'");
                        break;
                    }
                }
            }
            return attributeInstance;
        }
    }
}
