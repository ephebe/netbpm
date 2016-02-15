using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public class ExecutionContext :　IAssignmentContext, IDecisionContext, IForkContext, IActionContext, IJoinContext, IProcessInvocationContext, ITaskContext
    {
        public IFlow GetFlow()
        {
            throw new NotImplementedException();
        }

        #region IAssignmentContext
        public IOrganisationService GetOrganisationComponent()
        {
            throw new NotImplementedException();
        }

        public INode GetNode()
        {
            throw new NotImplementedException();
        }

        public IActor GetPreviousActor()
        {
            throw new NotImplementedException();
        }

        public IDictionary GetConfiguration()
        {
            throw new NotImplementedException();
        }

        public IProcessDefinition GetProcessDefinition()
        {
            throw new NotImplementedException();
        }

        public IProcessInstance GetProcessInstance()
        {
            throw new NotImplementedException();
        }

        public object GetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public void AddLog(string msg)
        {
            throw new NotImplementedException();
        }

        public void Schedule(Job job)
        {
            throw new NotImplementedException();
        }

        public void Schedule(Job job, string reference)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ForkContext
        public void ForkFlow(string transitionName)
        {
            throw new NotImplementedException();
        }

        public void ForkFlow(string transitionName, IDictionary attributeValues)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ActionContext
        public void SetAttribute(string attributeName, object attributeValue)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region JoinContext
        public IList GetOtherActiveConcurrentFlows()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ProcessInvocationContext
        public IActionContext GetInvokedProcessContext()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
