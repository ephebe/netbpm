using System;
using log4net;

namespace NetBpm.Workflow.Delegation.Impl
{
	public class AttributeExpressionResolver
	{
		private const String LEFT_MARKER = "${";
		private const String RIGHT_MARKER = "}";
		private static readonly ILog log = LogManager.GetLogger(typeof (AttributeExpressionResolver));
		private static readonly AttributeExpressionResolver instance = new AttributeExpressionResolver();

		public static AttributeExpressionResolver Instance
		{
			// the singleton pattern getInstance
			get { return instance; }
		}

		// the singleton pattern constructor
		private AttributeExpressionResolver()
		{
		}

		public String ResolveAttributeExpression(String expression, IHandlerContext handlerContext)
		{
			String text = expression;

			int leftMarkerIndex = text.IndexOf(LEFT_MARKER);
			int rightMarkerIndex = text.IndexOf(RIGHT_MARKER, leftMarkerIndex + LEFT_MARKER.Length);

			while ((leftMarkerIndex != - 1) && (rightMarkerIndex != - 1))
			{
				String attributeName = text.Substring(leftMarkerIndex + LEFT_MARKER.Length, (rightMarkerIndex) - (leftMarkerIndex + LEFT_MARKER.Length)).Trim();


				try
				{
					Object attribute = handlerContext.GetAttribute(attributeName);
					if (attribute != null)
					{
						String attributeString = attribute.ToString();
						text = text.Substring(0, leftMarkerIndex) + attributeString + text.Substring(rightMarkerIndex + RIGHT_MARKER.Length);
						rightMarkerIndex = rightMarkerIndex + attributeString.Length - attributeName.Length - LEFT_MARKER.Length - RIGHT_MARKER.Length;
					}
				}
				catch (Exception e)
				{
					log.Debug("attribute '" + attributeName + "' could not be resolved in attribute expression '" + expression + "'. Exception: " + e.Message);
				}

				leftMarkerIndex = text.IndexOf(LEFT_MARKER, rightMarkerIndex + RIGHT_MARKER.Length);
				rightMarkerIndex = text.IndexOf(RIGHT_MARKER, leftMarkerIndex + LEFT_MARKER.Length);
			}

			return text;
		}
	}
}