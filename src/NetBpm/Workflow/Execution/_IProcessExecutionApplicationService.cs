using NetBpm.Util.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Execution
{
    public interface IProcessExecutionApplicationService
    {
        IProcessInstance StartProcessInstance(Int64 processDefinitionId);
        IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues);
        IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName);
        IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations);
    }
}
