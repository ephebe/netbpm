using System;
using log4net;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Delegation.Impl.Assignment
{
	/// <summary> allows to specify the next-actor in a process definition as an expression using the following syntax.
	/// <p>The general syntax is firstArgument->nextArgument->nextArgument->...->nextArgument
	/// </p> 
	/// <p>For the firstArgument, following constructions are valid :
	/// <ul>
	/// <li><b>previousActor</b> : </li>
	/// <li><b>actor( &lt;actorName&gt; )</b> : </li>
	/// <li><b>user( &lt;userName&gt; )</b> : </li>
	/// <li><b>group( &lt;groupName&gt; )</b> : </li>
	/// </ul>
	/// </p> 
	/// <p>For the nextArgument's, following constructions are valid :
	/// <ul>
	/// <li><b>[User]->group( &lt;membership-type&gt; )</b> results in a Group</li>
	/// <li><b>[Group]->role( &lt;role&gt; )</b> results in a User</li>
	/// <li><b>[Group]->parentGroup</b> results in a Group</li>
	/// </ul>
	/// </p> 
	/// </summary>
	public class AssignmentExpressionResolver : IAssignmentHandler
	{
		private ActorExpressionResolver _actorExpressionResolver = ActorExpressionResolver.Instance;
		private static readonly ILog log = LogManager.GetLogger(typeof (AssignmentExpressionResolver));

		public String SelectActor(IAssignmentContext assignmentContext)
		{
			String actorId = null;

			String expression = (String) assignmentContext.GetConfiguration()["expression"];

			try
			{
				IActor actor = _actorExpressionResolver.ResolveArgument(expression, assignmentContext);
				if (actor != null)
				{
					actorId = actor.Id;
				}
			}
			catch (Exception e)
			{
				log.Error("error selecting an actor :", e);
				throw new SystemException(e.Message);
			}

			return actorId;
		}
	}
}