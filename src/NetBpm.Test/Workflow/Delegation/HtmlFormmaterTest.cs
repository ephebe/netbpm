using System;
using System.Collections;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Delegation.Impl.Htmlformatter;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Delegation
{
	/// <summary>
	/// Zusammenfassung für HtmlFormmaterTest.
	/// </summary>
	[TestFixture]
	public class HtmlFormmaterTest
	{
		public HtmlFormmaterTest()
		{
		}

		[Test]
		public void TestDateInput()
		{
			try
			{
				IHtmlFormatter dateInput = new DateInput();
				IDictionary configuration = new Hashtable();
				configuration.Add("dateFormat","dd/MM/yyyy");
				dateInput.SetConfiguration(configuration);
				DateTime date = (DateTime)dateInput.ParseHttpParameter("11/11/2004",null);
				Assert.IsNotNull(date);
				Assert.IsTrue(date.Year==2004);
				Assert.IsTrue(date.Month==11);
				Assert.IsTrue(date.Day==11);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}
