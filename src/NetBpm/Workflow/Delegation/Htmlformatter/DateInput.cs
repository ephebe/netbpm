using System;
using System.Globalization;
using log4net;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	
	public class DateInput:AbstractConfigurable, IHtmlFormatter
	{
		private static readonly ILog log = LogManager.GetLogger( typeof(DateInput) );
		private static readonly CultureInfo enUS = new CultureInfo("en-US", false);

		public String ObjectToHtml(Object valueObject, String parameterName, System.Web.HttpRequest request)
		{
			String html = null;
			String dateFormat=(String)GetConfiguration()["dateFormat"];
			log.Debug("dateformat: " + dateFormat);
						
			html = "<input type=text size=11 name=\"" + parameterName + "\"";
			
			if (valueObject != null)
			{
				html += (" value=\"" + ((DateTime) valueObject).ToString(dateFormat,enUS) + "\"");
			}
			
			html += ("> (" + dateFormat + ")");
			
			return html;
		}
		
		public Object ParseHttpParameter(String text, System.Web.HttpRequest request)
		{
			String dateFormat=(String)GetConfiguration()["dateFormat"];
			log.Debug("dateformat: " + dateFormat);
			if (((System.Object) text == null) || ("".Equals(text)))
			{
				return null;
			}
			return DateTime.ParseExact(text, dateFormat ,enUS);
		}
	}
}