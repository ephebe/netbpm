using System;
using log4net;

namespace NetBpm.Workflow.Log.Impl
{
	public class ExceptionReportImpl : LogDetailImpl, IExceptionReport
	{
		private String _exceptionClassName = null;
		private String _exceptionMessage = null;
		private String _stackTrace = null;

		private static readonly log4net.ILog log = LogManager.GetLogger(typeof (ExceptionReportImpl));

        public virtual String ExceptionClassName
		{
			get { return this._exceptionClassName; }
			set { this._exceptionClassName = value; }
		}

        public virtual String ExceptionMessage
		{
			get { return this._exceptionMessage; }
			set { this._exceptionMessage = value; }
		}

        public virtual String StackTrace
		{
			get { return this._stackTrace; }
			set { this._stackTrace = value; }
		}

		public ExceptionReportImpl()
		{
		}

		public ExceptionReportImpl(Exception t)
		{
			this._exceptionClassName = t.GetType().FullName;
			this._exceptionMessage = t.Message;
			//@portme
			/*			
			System.IO.StringWriter stringWriter = new System.IO.StringWriter();
			SupportClass.WriteStackTrace(t, new PrintWriter(stringWriter));
			this.stackTrace = stringWriter.ToString();*/
		}
	}
}