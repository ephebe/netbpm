using System;
using System.Collections;
using System.Reflection;
using log4net;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation;
using NHibernate.Util;

namespace NetBpm.Workflow.Delegation.Impl
{
	/// <summary> resolves an expression to a user or a group using the following syntax :
	/// <p>The general syntax is firstArgument->nextArgument->nextArgument->...->nextArgument
	/// </p> 
	/// <p>For the firstArgument, following constructions are valid :
	/// <ul>
	/// <li><b>previousActor</b> : </li>
	/// <li><b>processInitiator</b> : </li>
	/// <li><b>actor( &lt;actorName&gt; )</b> : </li>
	/// <li><b>role( &lt;attributeName&gt; )</b> : </li>
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
	public class ActorExpressionResolver
	{
		private static Type[] RESOLVE_METHOD_ARGUMENT_TYPES = new Type[] {typeof (IActor), typeof (String[]), typeof (ExecutionContextImpl)};
		private static readonly ActorExpressionResolver instance = new ActorExpressionResolver();
		private static readonly ILog log = LogManager.GetLogger(typeof (ActorExpressionResolver));

		/// <summary> gets the singleton instance.</summary>
		public static ActorExpressionResolver Instance
		{
			get { return instance; }
		}

		private ActorExpressionResolver()
		{
		}

		public IActor ResolveArgument(String expression, IHandlerContext handlerContext)
		{
			return ResolveArgument(null, expression, 0, handlerContext);
		}

		private IActor ResolveArgument(IActor resolvedActor, String expression, int index, IHandlerContext handlerContext)
		{
			String argument = null;

			log.Debug("resolving actor expression '" + expression + "',  resolvedActor is " + resolvedActor);
			String whiteSpace = "                                                                                      ".Substring(0, index);
			log.Debug("                            " + whiteSpace + "^");

			String[] parameters = null;

			int argumentEndIndex = expression.IndexOf("->", index);
			if (argumentEndIndex == - 1)
			{
				argumentEndIndex = expression.Length;
			}

			int parametersStartIndex = expression.IndexOf("(", index);
			int parametersEndIndex = - 1;
			if ((parametersStartIndex != - 1) && (parametersStartIndex < argumentEndIndex))
			{
				argument = expression.Substring(index, (parametersStartIndex) - (index)).Trim();
				parametersEndIndex = expression.IndexOf(")", parametersStartIndex + 1);

				if (parametersEndIndex > argumentEndIndex)
				{
					throw new SystemException("can't resolve assigner expression : couldn't find closing bracket for bracket on index '" + parametersStartIndex + "' in expression '" + expression + "'");
				}

				// the next exception happens when a parameter contains a right bracket.
				String shouldBewhiteSpace = expression.Substring(parametersEndIndex + 1, (argumentEndIndex) - (parametersEndIndex + 1));
				if (!"".Equals(shouldBewhiteSpace.Trim()))
				{
					throw new SystemException("can't resolve assigner expression : only whitespace allowed between closing bracket of the parameterlist of an argument and the end of the argument : closing bracket position '" + parametersEndIndex + "' in expression '" + expression + "'");
				}

				String parametersText = expression.Substring(parametersStartIndex + 1, (parametersEndIndex) - (parametersStartIndex + 1));
				ArrayList parameterList = new ArrayList();

				StringTokenizer tokenizer = new StringTokenizer(parametersText, ",");
				IEnumerator tokenEnum = tokenizer.GetEnumerator();
				while (tokenEnum.MoveNext())
				{
					parameterList.Add(tokenEnum.Current.ToString().Trim());
				}

				if (parameterList.Count > 0)
				{
					parameters = new String[parameterList.Count];
					parameters = (String[]) parameterList.ToArray(typeof (String));
				}
				else
				{
					parameters = new String[0];
				}
			}
			else
			{
				argument = expression.Substring(index, (argumentEndIndex) - (index)).Trim();
				parameters = new String[0];
			}

			if ("".Equals(argument))
			{
				throw new SystemException("can't resolve assigner expression : can't resolve empty argument on index '" + index + "' for expression '" + expression + "'");
			}

			String methodName = "ResolveArgument" + argument.Substring(0, 1).ToUpper() + argument.Substring(1);
			try
			{
				MethodInfo method = this.GetType().GetMethod(methodName, (Type[]) RESOLVE_METHOD_ARGUMENT_TYPES);
				Object[] args = new Object[] {resolvedActor, parameters, handlerContext};

				resolvedActor = (IActor) method.Invoke(this, (Object[]) args);
				log.Debug(methodName + " came up with " + resolvedActor);
			}
			catch (Exception t)
			{
				throw new SystemException("can't resolve assigner expression : couldn't resolve argument '" + argument + "' : " + t.Message,t);
			}


			if (argumentEndIndex != expression.Length)
			{
				if (argumentEndIndex < expression.Length)
				{
					argumentEndIndex = expression.IndexOf("->", argumentEndIndex) + 2;
				}
				resolvedActor = ResolveArgument(resolvedActor, expression, argumentEndIndex, handlerContext);
			}

			return resolvedActor;
		}

		public Object ResolveArgumentPreviousActor(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			if (parameters.Length != 0)
				throw new SystemException("argument previousActor expects exactly zero parameters instead of " + parameters.Length);
			IActor actor = executionContext.GetPreviousActor();
			if (actor == null)
				throw new SystemException("argument previousActor could not be resolve because there is no previous actor");
			return actor;
		}

		public Object ResolveArgumentProcessInitiator(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			return executionContext.GetProcessInstance().GetInitiator();
		}

		public Object ResolveArgumentActor(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			if (parameters.Length != 1)
				throw new SystemException("argument actor expects exactly one (the actor-id) parameter instead of " + parameters.Length);

			IActor actor = null;
			try
			{
				actor = executionContext.GetOrganisationComponent().FindActorById(parameters[0]);
			}
			catch (OrganisationRuntimeException e)
			{
				throw new SystemException("can't resolve actor-argument with parameter " + parameters[0], e);
			}

			return actor;
		}

		public Object ResolveArgumentGroup(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			log.Debug("resolvedActor inside resolveArgumentGroup: " + resolvedActor);

			if (resolvedActor == null)
			{
				if (parameters.Length != 1)
					throw new SystemException("argument group expects exactly one parameter instead of " + parameters.Length);

				String groupId = parameters[0];
				IGroup group = null;
				try
				{
					group = executionContext.GetOrganisationComponent().FindGroupById(groupId);
				}
				catch (OrganisationRuntimeException e)
				{
					throw new SystemException("can't resolve group-argument with parameter " + groupId + " : " + e.Message);
				}

				return group;
			}
			else
			{
				if (parameters.Length != 1)
					throw new SystemException("argument group expects exactly one parameter (membership-type) instead of " + parameters.Length);

				IUser user = null;
				IGroup group = null;
				String membershipType = parameters[0];

				try
				{
					group = executionContext.GetOrganisationComponent().FindGroupByMembership(resolvedActor.Id, membershipType);
				}
				catch (InvalidCastException e)
				{
					throw new SystemException("can't resolve group-argument : a group must be calculated from a User, not a " + resolvedActor.GetType().FullName, e);
				}
				catch (OrganisationRuntimeException e)
				{
					throw new SystemException("can't resolve group-argument : can't find the hierarchy-memberschip of User " + user.Id + " and membership-type " + membershipType + " : " + e.Message, e);
				}

				return group;
			}
		}

		public Object ResolveArgumentRole(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			if (parameters.Length != 1)
				throw new SystemException("argument role expects exactly one parameter (role-name) instead of " + parameters.Length);

			IActor actor = null;

			if (resolvedActor == null)
			{
				try
				{
					actor = (IActor) executionContext.GetAttribute(parameters[0]);
				}
				catch (InvalidCastException e)
				{
					throw new SystemException("argument attribute(" + parameters[0] + ") does not contain an actor : " + executionContext.GetAttribute(parameters[0]).GetType().FullName, e);
				}
			}
			else
			{
				String roleName = parameters[0].Trim();

				try
				{
					IList users = executionContext.GetOrganisationComponent().FindUsersByGroupAndRole(resolvedActor.Id, roleName);
					if (users.Count < 1)
						throw new SystemException("no users have role " + roleName + " for group " + resolvedActor.Id);
					actor = (IUser) users[0];

					// TODO : create a new group if more then one user is returned on the query...
				}
				catch (InvalidCastException e)
				{
					throw new SystemException("can't resolve role-argument : a role must be calculated from a Group, not a " + resolvedActor.GetType().FullName, e);
				}
				catch (OrganisationRuntimeException e)
				{
					throw new SystemException("can't resolve role-argument : can't find the users that perform role " + roleName + " for group " + resolvedActor.Id + " : " + e.Message);
				}
			}

			return actor;
		}

		public Object ResolveArgumentParentGroup(IActor resolvedActor, String[] parameters, ExecutionContextImpl executionContext)
		{
			if (parameters.Length != 0)
				throw new SystemException("argument parentGroup expects exactly zero parameters instead of " + parameters.Length);

			IGroup group = null;

			try
			{
				group = (IGroup) resolvedActor;
				group = group.Parent;
			}
			catch (InvalidCastException e)
			{
				throw new SystemException("can't resolve parentGroup-argument : a role must be calculated from a Group, not a " + resolvedActor.GetType().FullName, e);
			}

			return group;
		}
	}
}