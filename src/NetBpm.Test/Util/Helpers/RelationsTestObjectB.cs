using System;
using System.Collections;

namespace NetBpm.Test.Util.Helper
{
	/// <summary> </summary>
	public class RelationsTestObjectB
	{
		public RelationsTestObjectC SingleReturn
		{
			get { return new RelationsTestObjectC(); }

		}

		public ICollection CollectionReturn
		{
			get
			{
				ArrayList c = new ArrayList();
				c.Add(new RelationsTestObjectC());
				c.Add(new RelationsTestObjectD());
				c.Add(new RelationsTestObjectE());
				return c;
			}

		}

		public Object[] ArrayReturn
		{
			get
			{
				Object[] o = new Object[] {new RelationsTestObjectC(), new RelationsTestObjectD(), new RelationsTestObjectE()};
				return o;
			}

		}

		public RelationsTestObjectB()
		{
		}
	}
}