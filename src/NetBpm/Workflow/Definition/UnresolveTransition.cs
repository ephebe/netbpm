using NetBpm.Workflow.Definition.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Workflow.Definition
{
    /// <summary>
    /// 用來在解析XML過程中，暫時替代還沒找到Transition的To結點
    /// </summary>
    public class UnresolveTransition : TransitionImpl
    {
        public string XmlValue { get; set; }
    }
}
