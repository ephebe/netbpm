using System;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	
	public class DateLabel:AbstractConfigurable, IHtmlFormatter
	{
		public System.String ObjectToHtml(Object valueObject, String parameterName, System.Web.HttpRequest request)
		{
			System.String html = null;
			
			if (valueObject != null)
			{
				String dateFormat=(String)GetConfiguration()["dateFormat"];
				html = ((System.DateTime) valueObject).ToString(dateFormat) + " (" + dateFormat + ")";
			}
			else
			{
				html = "";
			}
			
			return html;
		}
		
		public Object ParseHttpParameter(String text, System.Web.HttpRequest request)
		{
			return null;
		}
	}
}