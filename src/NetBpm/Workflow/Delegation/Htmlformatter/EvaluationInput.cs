using System;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Definition.Attr;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	
	public class EvaluationInput:AbstractConfigurable, IHtmlFormatter
	{
		
		public String ObjectToHtml(System.Object valueObject, System.String parameterName, System.Web.HttpRequest request)
		{
			System.Text.StringBuilder htmlBuffer = new System.Text.StringBuilder();
						htmlBuffer.Append("<table border=0 cellspacing=0 cellpadding=0><tr><td nowrap style=\"background-color:transparent;\">");
			htmlBuffer.Append("<input type=radio name=\"");
			htmlBuffer.Append(parameterName);
			htmlBuffer.Append("\" value=\"");
			htmlBuffer.Append(Evaluation.APPROVE);
			htmlBuffer.Append("\">");
			htmlBuffer.Append("&nbsp; approve");
			htmlBuffer.Append("</td></tr><tr><td nowrap style=\"background-color:transparent;\">");
			htmlBuffer.Append("<input type=radio name=\"");
			htmlBuffer.Append(parameterName);
			htmlBuffer.Append("\" value=\"");
			htmlBuffer.Append(Evaluation.DISAPPROVE);
			htmlBuffer.Append("\">");
			htmlBuffer.Append("&nbsp; disapprove");
			htmlBuffer.Append("</td></tr></table>");
			
			return htmlBuffer.ToString();
		}
		
		public Object ParseHttpParameter(System.String text, System.Web.HttpRequest request)
		{
			System.Object evaluationResult = null;
			
			try
			{
				evaluationResult = Evaluation.ParseEvaluation(text);
			}
			catch (System.ArgumentException e)
			{
				throw new System.FormatException("couldn't parse the Evaluation from value " + text +" exception message: "+e.Message);
			}
			
			return evaluationResult;
		}
	}
}