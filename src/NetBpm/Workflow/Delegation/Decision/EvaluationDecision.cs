using System;
using NetBpm.Workflow.Definition.Attr;

namespace NetBpm.Workflow.Delegation.Impl.Decision
{
	public class EvaluationDecision : IDecisionHandler
	{
		public String Decide(IDecisionContext decisionContext)
		{
			String transitionName = "disapprove";

			String attributeName = (String) decisionContext.GetConfiguration()["attribute"];
			Object attributeValue = decisionContext.GetAttribute(attributeName);

			if (attributeValue == Evaluation.APPROVE)
			{
				transitionName = "approve";
			}

			return transitionName;
		}

        public String Decide(string attributeValue)
        {
            String transitionName = "disapprove";

            if (attributeValue == Evaluation.APPROVE.ToString())
            {
                transitionName = "approve";
            }

            return transitionName;
        }
	}
}