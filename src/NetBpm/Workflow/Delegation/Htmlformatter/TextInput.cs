using System;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	
	public class TextInput:AbstractConfigurable, IHtmlFormatter
	{
		
		public System.String ObjectToHtml(System.Object valueObject, System.String parameterName, System.Web.HttpRequest request)
		{
			System.String html = null;
			System.String size = "10";
			
			if (GetConfiguration().Contains("size"))
			{
				size = ((System.String) GetConfiguration()["size"]);
			}
			
			System.String text = "";
			if (valueObject != null)
				text = ((System.String) valueObject);
			
			html = "<input type=text size=" + size + " name=\"" + parameterName + "\" value=\"" + text + "\">";
			
			return html;
		}
		
		public System.Object ParseHttpParameter(System.String text, System.Web.HttpRequest request)
		{
			return text;
		}
	}
}