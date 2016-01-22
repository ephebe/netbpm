using Iesi.Collections;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Definition.Impl;
using NHibernate.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class TransitionRepository
    {
        private static readonly TransitionRepository instance = new TransitionRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof(TransitionRepository));

		/// <summary> gets the singleton instance.</summary>
		public static TransitionRepository Instance
		{
			get { return instance; }
		}

        private TransitionRepository()
		{
		}

        private const String queryFindTransitionByName = "select t " +
            "from t in class NetBpm.Workflow.Definition.Impl.TransitionImpl, " +
            "     s in class NetBpm.Workflow.Definition.Impl.StateImpl " +
            "where t.From = s.id " +
            "  and t.Name = ? " +
            "  and s.id = ? ";

        /* package private */

        internal virtual TransitionImpl GetTransition(String transitionName, StateImpl state, DbSession dbSession)
        {
            TransitionImpl transition = null;
            if ((Object)transitionName != null)
            {
                Object[] values = new Object[] { transitionName, state.Id };
                IType[] types = new IType[] { DbType.STRING, DbType.LONG };
                transition = (TransitionImpl)dbSession.FindOne(queryFindTransitionByName, values, types);
            }
            else
            {
                ISet leavingTransitions = state.LeavingTransitions;
                if (leavingTransitions.Count == 1)
                {
                    IEnumerator transEnum = leavingTransitions.GetEnumerator();
                    transEnum.MoveNext();
                    transition = (TransitionImpl)transEnum.Current;
                }
                else
                {
                    throw new SystemException("no transitionName was specified : this is only allowed if the state (" + state.Name + ") has exactly 1 leaving transition (" + leavingTransitions.Count + ")");
                }
            }
            return transition;
        }

        private const String queryFindLeavingTransitionByName = "select t " +
            "from t in class NetBpm.Workflow.Definition.Impl.TransitionImpl, " +
            "     n in class NetBpm.Workflow.Definition.Impl.NodeImpl " +
            "where n.id = ? " +
            "  and t.From.id = n.id " +
            "  and t.Name = ? ";

        internal virtual TransitionImpl FindLeavingTransitionByName(long nodeId,string transitionName, DbSession dbSession)
        {
            Object[] values = new Object[] { nodeId, transitionName };
            IType[] types = new IType[] { DbType.LONG, DbType.STRING };

            return (TransitionImpl)dbSession.FindOne(queryFindLeavingTransitionByName, values, types);
        }
    }
}
