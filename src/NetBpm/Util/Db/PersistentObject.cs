using System;

namespace NetBpm.Util.DB
{
	//@portme http://www.codeproject.com/csharp/objserial.asp
//#Serializable#
	public class PersistentObject //: System.Runtime.Serialization.ISerializable
	{
		protected Int64 _id;

        public virtual Int64 Id
		{
			get { return _id; }
			set { this._id = value; }
		}

		// toString
		public override String ToString()
		{
			String className = this.GetType().FullName;
			int to = className.Length;
			if (className.EndsWith("Impl"))
			{
				to = to - 4;
			}
			int from = className.LastIndexOf('.') + 1;
			className = className.Substring(from, to - from);
			return className + "[" + _id + "]";
		}

	}
}