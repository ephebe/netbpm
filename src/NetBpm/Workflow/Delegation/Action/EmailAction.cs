using System;
using System.Collections;
using log4net;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Delegation.Impl.Action
{
	/// <summary> sends an email.</summary>
	public class EmailAction : IActionHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (EmailAction));
		private static readonly AttributeExpressionResolver _attributeExpressionResolver;
		private static readonly ActorExpressionResolver _actorExpressionResolver;

		static EmailAction()
		{
			_attributeExpressionResolver = AttributeExpressionResolver.Instance;
			_actorExpressionResolver = ActorExpressionResolver.Instance;
		}

		public void Run(IActionContext actionContext)
		{
			IDictionary configuration = actionContext.GetConfiguration();
			String subject = (String) configuration["subject"];
			String message = (String) configuration["message"];
			String from = (String) configuration["from"];
			String to = (String) configuration["to"];

			// resolving the texts
			subject = _attributeExpressionResolver.ResolveAttributeExpression(subject, actionContext);
			message = _attributeExpressionResolver.ResolveAttributeExpression(message, actionContext);
			IUser user = (IUser) _actorExpressionResolver.ResolveArgument(to, actionContext);
			to = user.Email;
			if ((Object) from == null)
			{
				from = actionContext.GetProcessDefinition().Name;
				from = from.ToLower();
				from = from.Replace(' ', '.');
				from += "@netbpm.org";
			}

			SendMail(from, to, subject, message, actionContext);
		}

		public void SendMail(String from, String to, String subject, String body, IActionContext interactionContext)
		{
			log.Info("sending mail from '+ from +'to '" + to + "' with subject '" + subject + "' and body '" + body + "'");

		}
	}
}