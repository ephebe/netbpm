using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition.Impl
{
    public interface IProcessDefinitionBuilder
    {
        ProcessDefinitionImpl ProcessDefinition { get;}
        ProcessBlockImpl ProcessBlock { get;}
        ProcessBlockImpl TransitionDestinationScope { get;set;}
        DefinitionObjectImpl DefinitionObject { get;set;}
        StateImpl State { get;set;}
        NodeImpl Node { get;set;}
        Object DelegatingObject { get;set;}
        int Index { get;set;}
        IDictionary<string, byte[]> Entries { get;}
    }
}
