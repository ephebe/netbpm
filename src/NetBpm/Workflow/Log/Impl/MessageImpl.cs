using System;

namespace NetBpm.Workflow.Log.Impl
{
	public class MessageImpl : LogDetailImpl, IMessage
	{
		private String _message = null;

        public virtual String MessageText
		{
			get { return this._message; }
			set { this._message = value; }
		}

		public MessageImpl()
		{
		}

		public MessageImpl(String message)
		{
			this._message = message;
		}
	}
}