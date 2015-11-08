using System;

namespace NetBpm.Util.DB
{
	public class DbException : SystemException
	{
		public DbException()
		{
		}

		public DbException(String msg) : base(msg)
		{
		}
	}
}