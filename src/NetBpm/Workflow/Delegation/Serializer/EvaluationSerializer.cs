using System;
using log4net;
using NetBpm.Workflow.Definition.Attr;

namespace NetBpm.Workflow.Delegation.Impl.Serializer
{
	public class EvaluationSerializer : AbstractConfigurable, ISerializer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (EvaluationSerializer));

		public String Serialize(Object object_Renamed)
		{
			String serialized = null;

			if (!(object_Renamed is Evaluation))
			{
				throw new ArgumentException("EvaluationSerializer can't serialize " + object_Renamed);
			}

			if (object_Renamed != null)
			{
				serialized = object_Renamed.ToString();
			}

			return serialized;
		}

		public Object Deserialize(String text)
		{
			if ((Object) text == null)
				return null;

			Evaluation evaluation = null;
			;
			try
			{
				evaluation = Evaluation.ParseEvaluation(text);
			}
			catch (FormatException e)
			{
				throw new ArgumentException("can't deserialize " + text + " to an Evaluation. " + e.Message);
			}
			return evaluation;
		}
	}
}