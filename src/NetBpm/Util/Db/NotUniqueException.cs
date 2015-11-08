using System;

namespace NetBpm.Util.DB
{
	public class NotUniqueException : DbException
	{
		private int nbrOfObjectsFound;
		private String query = null;
		private Object[] values = null;

		public int NbrOfObjectsFound
		{
			get { return this.nbrOfObjectsFound; }
			set { this.nbrOfObjectsFound = value; }
		}

		public String Query
		{
			get { return this.query; }
			set { this.query = value; }
		}

		public Object[] Values
		{
			get { return this.values; }
			set { this.values = value; }
		}

		public NotUniqueException(String query, int nbrOfObjectsFound) : base("query returned " + nbrOfObjectsFound + " Objects instead of only 1 : " + query)
		{
			this.query = query;
		}

		public NotUniqueException(String query, Object valueObject, int nbrOfObjectsFound) : base("query returned " + nbrOfObjectsFound + " Objects instead of only 1 : " + query + " : " + valueObject)
		{
			this.query = query;
			this.values = new Object[] {valueObject};
		}

		public NotUniqueException(String query, Object[] values, int nbrOfObjectsFound) : base("query returned " + nbrOfObjectsFound + " Objects instead of only 1 : " + query + " : " + values)
		{
			this.query = query;
			this.values = values;
		}
	}
}