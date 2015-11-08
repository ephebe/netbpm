using System;
using System.Collections;

namespace NetBpm.Test.Util.Helper
{
	/// <summary> Created to test Relations. Relation test object A
	/// 
	/// </summary>
	public class RelationsTestObjectA
	{
		private bool lazySingleReturn = true;
		private bool lazyCollectionReturn = true;
		private bool lazyArrayReturn = true;
		
		public bool LazyCollectionReturn
		{
			get {return lazyCollectionReturn;}
		}

		public bool LazySingleReturn
		{
			get {return lazySingleReturn;}
		}

		public bool LazyArrayReturn
		{
			get {return lazyArrayReturn;}
		}

		public RelationsTestObjectB SingleReturn
		{
			get {
				lazySingleReturn=false;
				return (new RelationsTestObjectB()); 
			}

		}

		public ICollection CollectionReturn
		{
			get
			{
				lazyCollectionReturn=false;
				ArrayList c = new ArrayList();
				c.Add(new RelationsTestObjectB());
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
				lazyArrayReturn=false;
				Object[] o = new Object[] {new RelationsTestObjectB(), new RelationsTestObjectC(), new RelationsTestObjectD(), new RelationsTestObjectE()};
				return o;
			}

		}

		public RelationsTestObjectA()
		{
		}
	}
}