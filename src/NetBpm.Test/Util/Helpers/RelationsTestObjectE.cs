using System;
using System.Collections;

namespace NetBpm.Test.Util.Helper
{
	/// <summary> </summary>
	public class RelationsTestObjectE
	{
		public RelationsTestObjectE SingleReturn
		{
			get { return new RelationsTestObjectE(); }

		}

		public ICollection CollectionReturn
		{
			get
			{
				ArrayList c = new ArrayList();
				c.Add(new RelationsTestObjectE());
				return c;
			}

		}

		public Object[] ArrayReturn
		{
			get
			{
				Object[] o = new Object[] {new RelationsTestObjectE()};
				return o;
			}

		}

		public RelationsTestObjectE()
		{
		}
	}
}