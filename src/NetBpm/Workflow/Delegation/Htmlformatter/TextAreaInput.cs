using System;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	
	public class TextAreaInput:AbstractConfigurable, IHtmlFormatter
	{
		private static readonly ILog log = LogManager.GetLogger( typeof(TextAreaInput) );
		
		public String ObjectToHtml(Object valueObject, String parameterName, System.Web.HttpRequest request)
		{
			System.String html = null;
			
			System.String cols = (String) GetConfiguration()["cols"];
			System.String rows = (String) GetConfiguration()["rows"];
			
			html = "<textarea rows=" + rows + " cols=" + cols + " name=\"" + parameterName + "\">";
			
			if (valueObject != null)
			{
				html += valueObject.ToString();
			}
			
			html += "</textarea>";
			
			log.Debug("generated text area control : " + html);
			
			return html;
		}
		
		public Object ParseHttpParameter(String text, System.Web.HttpRequest request)
		{
			return text;
		}
	}
}