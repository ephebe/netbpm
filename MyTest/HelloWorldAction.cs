using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using NetBpm.Workflow.Delegation;

namespace MyTest
{
    public class HelloWorldAction : IActionHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HelloWorldAction));

        public void Run(IActionContext actionContext)
        {
            actionContext.SetAttribute("the text attrib",":(");
        }
    }
}
