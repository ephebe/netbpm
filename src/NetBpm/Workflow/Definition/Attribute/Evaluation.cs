using System;

namespace NetBpm.Workflow.Definition.Attr
{
	/// <summary> is a frequently used Attribute-type, it is used for representing the result of 
	/// an approval decision. The {@link Nbpm.Workflow.Delegation.ISerializer} for 
	/// Evaluation-objects is NetBpm.Workflow.Delegation.Impl.Serializer.EvaluationSerializer.
	/// To convert Evaluation-objects in NetBpm's web-interface to and from html, use 
	/// NetBpm.Workflow.Delegation.Impl.Htmlformatter.EvaluationInput as
	/// {@link Nbpm.Workflow.Delegation.IHtmlFormatter}  </summary>
	// @portme [Serializable]
	public sealed class Evaluation //: System.Runtime.Serialization.ISerializable
	{
		public static readonly Evaluation APPROVE = new Evaluation("approve");
		public static readonly Evaluation DISAPPROVE = new Evaluation("disapprove");
		private String _name = null;

		public static Evaluation ParseEvaluation(String text)
		{
			if (text == null)
				return null;

			if (text.ToUpper().Equals(APPROVE.ToString().ToUpper()))
			{
				return APPROVE;
			}
			else
			{
				if (text.ToUpper().Equals(DISAPPROVE.ToString().ToUpper()))
				{
					return DISAPPROVE;
				}
				else
				{
					throw new FormatException("Couldn't parse " + text + " to a valid EvaluationResult");
				}
			}
		}

		private Evaluation(String name)
		{
			this._name = name;
		}

		public override String ToString()
		{
			return _name;
		}

/*		private Object ReadResolve()
		{
			Object singletonObject = null;
			try
			{
				singletonObject = ParseEvaluation(_name);
			}
			catch (System.FormatException e)
			{
				throw new System.IO.IOException("couldn't resolve evaluation '" + _name + "' during deserialization",e);
			}
			return singletonObject;
		}*/
	}
}