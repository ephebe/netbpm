using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Impl;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution.Impl;
using NHibernate.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Delegation
{
    public class DelegationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DelegationService));
        private static readonly DelegationHelper delegationHelper = DelegationHelper.Instance;

        private const String queryFindActionsByEventType = "select a from a in class NetBpm.Workflow.Definition.Impl.ActionImpl " +
            "where a.EventType = ? " +
            "  and a.DefinitionObjectId = ? ";

        public void RunActionsForEvent(EventType eventType, long definitionObjectId, ExecutionContextImpl executionContext,DbSession dbSession)
        {
            log.Debug("processing '" + eventType + "' events for executionContext " + executionContext);

            // find all actions for definitionObject on the given eventType
            Object[] values = new Object[] { eventType, definitionObjectId };
            IType[] types = new IType[] { DbType.INTEGER, DbType.LONG };

            IList actions = dbSession.Find(queryFindActionsByEventType, values, types);
            IEnumerator iter = actions.GetEnumerator();
            log.Debug("list" + actions);
            while (iter.MoveNext())
            {
                ActionImpl action = (ActionImpl)iter.Current;
                log.Debug("action: " + action);
                delegationHelper.DelegateAction(action.ActionDelegation, executionContext);
            }
            log.Debug("ende runActionsForEvent!");
        }
    }
}
