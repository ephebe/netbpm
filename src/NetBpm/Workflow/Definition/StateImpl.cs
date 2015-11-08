using System;
using System.Collections;
using Iesi.Collections;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Util.Xml;
using NHibernate.Type;

namespace NetBpm.Workflow.Definition.Impl
{
	public class StateImpl : NodeImpl, IState
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (StateImpl));
		private ISet _fields = null;
		private Int32 _x1;
		private Int32 _y1;
		private Int32 _x2;
		private Int32 _y2;

        public virtual ISet Fields
		{
			get { return this._fields; }
			set { this._fields = value; }
		}

		private Int32 CoordinateX1
		{
			get { return _x1; }
			set { _x1 = value; }
		}

		private Int32 CoordinateY1
		{
			get { return _y1; }
			set { _y1 = value; }
		}

		private Int32 CoordinateX2
		{
			get { return _x2; }
			set { _x2 = value; }
		}

		private Int32 CoordinateY2
		{
			get { return _y2; }
			set { _y2 = value; }
		}

        public virtual Int32[] ImageCoordinates
		{
			set
			{
				this._x1 = value[0];
				this._y1 = value[1];
				this._x2 = value[2];
				this._y2 = value[3];
			}
			get { return new Int32[] {_x1, _y1, _x2, _y2}; }
		}

		public StateImpl()
		{
		}

		public StateImpl(IProcessDefinition processDefinition) : base(processDefinition)
		{
			this._fields = new ListSet();
		}

        public virtual FieldImpl CreateField()
		{
			FieldImpl field = new FieldImpl();
			field.State = this;
			_fields.Add(field);
			return field;
		}

		public override void ReadProcessData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			base.ReadProcessData(xmlElement, creationContext);

			creationContext.State = this;
			this._fields = new ListSet();
			IEnumerator iter = xmlElement.GetChildElements("field").GetEnumerator();
			while (iter.MoveNext())
			{
				FieldImpl field = new FieldImpl();
				field.ReadProcessData((XmlElement) iter.Current, creationContext);
				_fields.Add(field);
			}
			creationContext.State = null;
		}

		private const String queryFindFieldByActivityStateAndAttributeName = "select f " +
			"from f in class NetBpm.Workflow.Definition.Impl.FieldImpl " +
			"where f.State.Id = ? " +
			"  and f.Attribute.Name = ?";
		private const String queryFindAttibuteByName = "select a " +
			"from a in class NetBpm.Workflow.Definition.Impl.AttributeImpl " +
			"where a.Scope.id = ? " +
			"  and a.Name = ?";

		public virtual void ReadWebData(XmlElement xmlElement, ProcessDefinitionBuildContext creationContext)
		{
			XmlElement coordinatesXmlElement = xmlElement.GetChildElement("image-coordinates");
			if (coordinatesXmlElement != null)
			{
				try
				{
					_x1 = Int32.Parse(coordinatesXmlElement.GetProperty("x1"));
					_y1 = Int32.Parse(coordinatesXmlElement.GetProperty("y1"));
					_x2 = Int32.Parse(coordinatesXmlElement.GetProperty("x2"));
					_y2 = Int32.Parse(coordinatesXmlElement.GetProperty("y2"));

					creationContext.Check((((Object) _x1 != null) && ((Object) _y1 != null) && ((Object) _x2 != null) && ((Object) _y2 != null)), "at least one of the image-coordinates (x1,y1,x2,y2) is missing : " + xmlElement);
				}
				catch (FormatException e)
				{
					creationContext.AddError("at least one of the image-coordinates is not parsable : " + xmlElement + " exception:" + e.Message);
				}
			}

			DbSession dbSession = creationContext.DbSession;
			creationContext.State = this;
			creationContext.Index = 0;
			IEnumerator iter = xmlElement.GetChildElements("field").GetEnumerator();
			while (iter.MoveNext())
			{
				XmlElement fieldElement = (XmlElement) iter.Current;
				String attributeName = fieldElement.GetProperty("attribute");

				FieldImpl field = null;

				Object[] values = new Object[] {_id, attributeName};
				IType[] types = new IType[] {DbType.LONG, DbType.STRING};

				IList fields = dbSession.Find(queryFindFieldByActivityStateAndAttributeName, values, types);
				if (fields.Count == 1)
				{
					field = (FieldImpl) fields[0];
				}
				else
				{
					values = new Object[] {_processBlock.Id, attributeName};
					types = new IType[] {DbType.LONG, DbType.STRING};
					AttributeImpl attribute = (AttributeImpl) dbSession.FindOne(queryFindAttibuteByName, values, types);

					field = new FieldImpl();
					field.Access = FieldAccess.READ_WRITE;
					field.State = this;
					field.Attribute = attribute;
					this._fields.Add(field);
				}

				field.ReadWebData(fieldElement, creationContext);
				creationContext.IncrementIndex();
			}
		}

		public override void Validate(ValidationContext validationContext)
		{
			base.Validate(validationContext);
		}
	}
}