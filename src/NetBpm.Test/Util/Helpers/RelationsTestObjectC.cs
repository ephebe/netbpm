using System;
using System.Collections;

namespace NetBpm.Test.Util.Helper
{
	/// <summary> 
	/// 
	/// </summary>
	public class RelationsTestObjectC
	{
		public RelationsTestObjectD SingleReturn
		{
			get { return new RelationsTestObjectD(); }

		}

		public ICollection CollectionReturn
		{
			get
			{
				ArrayList c = new ArrayList();
				c.Add(new RelationsTestObjectD());
				c.Add(new RelationsTestObjectE());
				return c;
			}

		}

		public Object[] ArrayReturn
		{
			get
			{
				Object[] o = new Object[] {new RelationsTestObjectD(), new RelationsTestObjectE()};
				return o;
			}

		}

		public RelationsTestObjectC()
		{
		}
	}
}