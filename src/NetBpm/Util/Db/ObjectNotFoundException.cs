using System;

namespace NetBpm.Util.DB
{
	public class ObjectNotFoundException : DbException
	{
		private Type _objectType = null;
		private object _id = null;
		private String _query = null;
		private Object[] _values = null;

		public Type ObjectType
		{
			get { return this._objectType; }
			set { this._objectType = value; }
		}

		public object Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public String Query
		{
			get { return this._query; }
			set { this._query = value; }
		}

		public Object[] Values
		{
			get { return this._values; }
			set { this._values = value; }
		}

		public ObjectNotFoundException(String query) : base("no object in database for query '" + query + "'")
		{
			this._query = query;
		}

		public ObjectNotFoundException(String query, Object valueObject) : base("no object in database for query '" + query + "' : " + valueObject)
		{
			this._query = query;
			this._values = new Object[] {valueObject};
		}

		public ObjectNotFoundException(String query, Object[] values) : base("no object in database for query '" + query + "' : " + values)
		{
			this._query = query;
			this._values = values;
		}

		public ObjectNotFoundException(Type objectType, object id) : base(objectType.FullName + " not found in database with id '" + id + "'")
		{
			this._objectType = objectType;
			this._id = id;
		}
	}
}