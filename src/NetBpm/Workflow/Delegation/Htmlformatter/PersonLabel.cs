using System;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Delegation.Impl.Htmlformatter
{
	public class PersonLabel:AbstractConfigurable, IHtmlFormatter
	{		
		public System.String ObjectToHtml(Object valueObject, String parameterName, System.Web.HttpRequest request)
		{
			System.String html = "";
			
			if (valueObject != null)
			{
				IUser user = (IUser) valueObject;
				html = user.Name;
			}
			
			return html;
		}
		
		public Object ParseHttpParameter(String text, System.Web.HttpRequest request)
		{
			return null;
		}
	}
}