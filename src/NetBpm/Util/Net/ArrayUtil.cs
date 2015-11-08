using System;
using System.Text;

namespace NetBpm.Util.Net
{
	/// <summary> is a utility class for common Array-methods that are not available in the system libraries. </summary>
	public class ArrayUtil
	{
		/// <summary> writes an Array to a String.</summary>
		public static String ToString(Object[] array)
		{
			if (array == null)
				return "null";
			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			String separator = null;
			for (int i = 0; i < array.Length; i++)
			{
				if ((Object) separator == null)
				{
					separator = "|";
				}
				else
				{
					sb.Append(separator);
				}
				sb.Append(array[i]);
			}
			sb.Append(']');
			return sb.ToString();
		}
	}
}