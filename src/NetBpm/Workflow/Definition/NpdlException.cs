using System;
using System.Collections;

namespace NetBpm.Workflow.Definition
{
	/// <summary> is the checked exception that is thrown when a process archive
	/// is not compliant as specified in the NetBpm Process Defintition Language (nPdl).
	/// A JpdlException tries to collect as many error messages in one parsing or
	/// validation, analogue to a compiler.  This allows a process developer to 
	/// correct more then one problem before redeploying the process archive.  
	/// Each message is optionally composed of a list of sub-messages separated 
	/// by colons ':'. The sub-messages are ordered from general to specific.
	/// </summary>
	/// <seealso href="http://www.netbpm.org/docs/npdl.html">
	/// </seealso>
	public class NpdlException : Exception
	{
		public IList ErrorMsgs
		{
			get { return errorMsgs; }
		}

		public NpdlException(String msg, Exception innerException) : base(msg, innerException)
		{
			this.errorMsgs = new ArrayList(1);
			this.errorMsgs.Add(msg);
		}

		public NpdlException(String msg) : base(msg)
		{
			this.errorMsgs = new ArrayList(1);
			this.errorMsgs.Add(msg);
		}

		public NpdlException(IList errorMsgs) : base(errorMsgs.ToString())
		{
			this.errorMsgs = errorMsgs;
		}

		private IList errorMsgs = null;
	}
}