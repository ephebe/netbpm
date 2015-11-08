using System;
using NetBpm.Workflow.Definition;

namespace NetBpm.Web.Presentation.Model
{
	public class FormRow
	{
		private String _generatedHtml;
		private IField _field;

		public FormRow(IField field,String html)
		{
			_field=field;
			_generatedHtml=html;
		}

		public String GeneratedHtml
		{
			get { return this._generatedHtml; }
			set { this._generatedHtml = value; }
		}

		public IField Field
		{
			get { return this._field; }
			set { this._field = value; }
		}

		public bool IsRequired()
		{
			return FieldAccessHelper.IsRequired(_field.Access);
		}
	}
}
