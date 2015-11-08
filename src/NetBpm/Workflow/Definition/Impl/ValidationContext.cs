using System;
using System.Collections;
using System.Text;
using log4net;

namespace NetBpm.Workflow.Definition.Impl
{
	public class ValidationContext
	{
		private IList _errors = new ArrayList();
		private IList _scope = new ArrayList();
		private static readonly ILog log = LogManager.GetLogger(typeof (ProcessDefinitionBuildContext));

		public ValidationContext()
		{
		}

		public IList Errors
		{
			get { return this._errors; }

			set { this._errors = value; }

		}

		public void AddError(String errorMsg)
		{
			log.Error(errorMsg);
			this._errors.Add(errorMsg);
		}

		public bool HasErrors()
		{
			return (_errors.Count > 0);
		}

		public void Check(bool condition, String errorMsg)
		{
			if (!condition)
			{
				StringBuilder buffer = new StringBuilder();
				IEnumerator iter = _scope.GetEnumerator();
				while (iter.MoveNext())
				{
					buffer.Append(iter.Current);
					buffer.Append(" : ");
				}
				buffer.Append(errorMsg);
				log.Error(buffer.ToString());
				_errors.Add(buffer.ToString());
			}
		}

		public void PushScope(String msg)
		{
			_scope.Add(msg);
		}

		public void PopScope()
		{
			_scope.Remove(_scope.Count);
		}
	}
}