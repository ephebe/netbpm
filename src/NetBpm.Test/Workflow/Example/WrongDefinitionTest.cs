using System;
using System.IO;
using NetBpm.Workflow.Definition;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
	[TestFixture]
	public class WrongDefinitionTest : AbstractTest
	{

		protected String GetParArchiv()
		{
			return "pme.par";
		}

		[Test]
		public void DeployProcess()
		{
			try
			{
				testUtil = new TestUtil();
				testUtil.LoginUser("ae");

				// deploy Archiv
				FileInfo parFile = new FileInfo(TestHelper.GetExampleDir()+GetParArchiv());
				FileStream fstream = parFile.OpenRead();
				byte[] b = new byte[parFile.Length];
				fstream.Read(b, 0, (int) parFile.Length);
				definitionComponent.DeployProcessArchive(b);
				Assert.Fail("where is my Exception!");
			} catch (NpdlException npdeEx)
			{
				
			}
		}
	}
}