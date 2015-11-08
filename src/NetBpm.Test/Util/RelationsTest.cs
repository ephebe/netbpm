using System;
using System.Collections;
using log4net;
using NetBpm.Test.Util.Helper;
using NetBpm.Util.Client;
using NUnit.Framework;

namespace NetBpm.Test.Util
{
	/// <summary> Test Relations. Used helper test object (RelationsTestObjectA, 
	/// RelationsTestObjectB, RelationsTestObjectC, RelationsTestObjectD,
	/// RelationsTestObjectE). Please add more test case if necessary.
	/// 
	/// A better way is to have all those test objects as inner class of RelationsTest
	/// but the Relations is to invoke method on classes not inner classes 
	/// (NoSuchMethodException would results). Relations could be modified to invoke 
	/// also inner class's method but that would defeat its purpose of being used to 
	/// resolve methods invocation of hibernate's POJO's lazy loading issues. If those
	/// test are in the same file with default accesibility IllegalAccess exception 
	/// might occurred as i don't think accessing default class is allowed. But one could
	/// try to grant the appropriate privillege, i suppose. Again that would be too much
	/// of work for just a test, so i came to the conclusion to have the test object in
	/// individual class.
	/// 
	/// </summary>
	[TestFixture]
	public class RelationsTest
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (RelationsTest));


		private Relations simpleSingleChainRelations1;
		private Relations simpleSingleChainRelations2;
		private Relations singleChainRelations;
		private String[] resultSimpleSingleChainRelations1;
		private String[] resultSimpleSingleChainRelations2;
		private String[] resultSingleChainRelations;

		private Relations simpleCollectionChainRelations1;
		private Relations simpleCollectionChainRelations2;
		private Relations collectionChainRelations;
		private String[] resultSimpleCollectionChainRelations1;
		private String[] resultSimpleCollectionChainRelations2;
		private String[] resultCollectionChainRelations;


		private Relations simpleArrayChainRelations1;
		private Relations simpleArrayChainRelations2;
		private Relations arrayChainRelations;
		private String[] resultSimpleArrayChainRelations1;
		private String[] resultSimpleArrayChainRelations2;
		private String[] resultArrayChainRelations;


		/// <summary>Creates a new instance of RelationsTest </summary>
		public RelationsTest()
		{
		}

		[SetUp]
		public void SetUp()
		{
			simpleSingleChainRelations1 = new Relations("singleReturn");
			simpleSingleChainRelations2 = new Relations("singleReturn.singleReturn");
			singleChainRelations = new Relations("singleReturn.singleReturn.singleReturn.singleReturn");
			resultSimpleSingleChainRelations1 = new String[] {"singleReturn"};
			resultSimpleSingleChainRelations2 = new String[] {"singleReturn", "singleReturn"};
			resultSingleChainRelations = new String[] {"singleReturn", "singleReturn", "singleReturn", "singleReturn"};


			simpleCollectionChainRelations1 = new Relations("collectionReturn");
			simpleCollectionChainRelations2 = new Relations("collectionReturn.singleReturn");
			collectionChainRelations = new Relations("collectionReturn.collectionReturn.collectionReturn.singleReturn");
			resultSimpleCollectionChainRelations1 = new String[] {"collectionReturn"};
			resultSimpleCollectionChainRelations2 = new String[] {"collectionReturn", "singleReturn"};
			resultCollectionChainRelations = new String[] {"collectionReturn", "collectionReturn", "collectionReturn", "singleReturn"};


			simpleArrayChainRelations1 = new Relations("arrayReturn");
			simpleArrayChainRelations2 = new Relations("arrayReturn.singleReturn");
			arrayChainRelations = new Relations("arrayReturn.arrayReturn.arrayReturn.singleReturn");
			resultSimpleArrayChainRelations1 = new String[] {"arrayReturn"};
			resultSimpleArrayChainRelations2 = new String[] {"arrayReturn", "singleReturn"};
			resultArrayChainRelations = new String[] {"arrayReturn", "arrayReturn", "arrayReturn", "singleReturn"};
		}

		[TearDown]
		public void TearDown()
		{
			simpleSingleChainRelations1 = null;
			simpleSingleChainRelations2 = null;
			singleChainRelations = null;
			resultSimpleSingleChainRelations1 = null;
			resultSimpleSingleChainRelations2 = null;
			resultSingleChainRelations = null;

			simpleCollectionChainRelations1 = null;
			simpleCollectionChainRelations2 = null;
			collectionChainRelations = null;
			resultSimpleCollectionChainRelations1 = null;
			resultSimpleCollectionChainRelations2 = null;
			resultCollectionChainRelations = null;

			simpleArrayChainRelations1 = null;
			simpleArrayChainRelations2 = null;
			arrayChainRelations = null;
			resultSimpleArrayChainRelations1 = null;
			resultSimpleArrayChainRelations2 = null;
			resultArrayChainRelations = null;
		}

		/// <summary> Test resolving single relation chain.</summary>
		[Test]
		public void TestResolveSimpleSingleChainRelations1()
		{
			RelationsTestObjectA relationsTestObjectA=new RelationsTestObjectA();
			simpleSingleChainRelations1.Resolve(relationsTestObjectA);
			CheckProperties(simpleSingleChainRelations1, resultSimpleSingleChainRelations1, 0);
			Assert.IsFalse(relationsTestObjectA.LazySingleReturn);
		}

		[Test]
		public void TestResolveSimpleSingleChainRelations2()
		{
			simpleSingleChainRelations2.Resolve(new RelationsTestObjectA());
			CheckProperties(simpleSingleChainRelations2, resultSimpleSingleChainRelations2, 0);
		}

		[Test]
		public void TestResolveSingleChainRelations()
		{
			singleChainRelations.Resolve(new RelationsTestObjectA());
			CheckProperties(singleChainRelations, resultSingleChainRelations, 0);
		}


		/// <summary> Test resolving collection relation chain</summary>
		[Test]
		public void TestResolveSimpleCollectionChainRelations1()
		{
			RelationsTestObjectA relationsTestObjectA=new RelationsTestObjectA();
			simpleCollectionChainRelations1.Resolve(relationsTestObjectA);
			CheckProperties(simpleCollectionChainRelations1, resultSimpleCollectionChainRelations1, 0);
			Assert.IsFalse(relationsTestObjectA.LazyCollectionReturn);

		}

		[Test]
		public void TestResolveSimpleCollectionChainRelations2()
		{
			simpleCollectionChainRelations2.Resolve(new RelationsTestObjectA());
			CheckProperties(simpleCollectionChainRelations2, resultSimpleCollectionChainRelations2, 0);
		}

		[Test]
		public void TestResolveCollectionChainRelations()
		{
			collectionChainRelations.Resolve(new RelationsTestObjectA());
			CheckProperties(collectionChainRelations, resultCollectionChainRelations, 0);
		}


		/// <summary> Test resolving array relation chain</summary>
		[Test]
		public void TestResolveSimpleArrayChainRelations1()
		{
			RelationsTestObjectA relationsTestObjectA=new RelationsTestObjectA();
			simpleArrayChainRelations1.Resolve(relationsTestObjectA);
			CheckProperties(simpleArrayChainRelations1, resultSimpleArrayChainRelations1, 0);
			Assert.IsFalse(relationsTestObjectA.LazyArrayReturn);
		}

		[Test]
		public void TestResolveSimpleArrayChainRelations2()
		{
			simpleArrayChainRelations2.Resolve(new RelationsTestObjectA());
			CheckProperties(simpleArrayChainRelations2, resultSimpleArrayChainRelations2, 0);
		}

		[Test]
		public void TestResolveArrayChainRelations()
		{
			arrayChainRelations.Resolve(new RelationsTestObjectA());
			CheckProperties(arrayChainRelations, resultArrayChainRelations, 0);
		}

		/// <summary> Check the properties if it is as declared in the result</summary>
		private void CheckProperties(Relations relations, String[] propertyResults, int propertyResultsIndex)
		{
			IEnumerator itr = relations.RelationsMap.GetEnumerator();
			while (itr.MoveNext())
			{
				DictionaryEntry mapEntry = (DictionaryEntry) itr.Current;
				String property = (String) mapEntry.Key;
				Assert.AreEqual(property, propertyResults[propertyResultsIndex]);
				if ((propertyResults.Length - 1) > propertyResultsIndex)
				{
					if (mapEntry.Value != null)
					{
						CheckProperties((Relations) mapEntry.Value, propertyResults, ++propertyResultsIndex);
					}
				}
			}
		}
	}
}