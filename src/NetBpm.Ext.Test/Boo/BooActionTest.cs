using System;
using System.Collections;
using NetBpm.Boo.Test;
using NetBpm.Ext.Boo;
using NetBpm.Workflow.Definition.Attr;
using NUnit.Framework;

namespace NetBpm.Ext.Test.Boo
{
	[TestFixture]
	public class BooActionTest
	{
		private BooAction action = null;
		private TestExecutionContextImpl context = null;

		[SetUp]
		public void SetUp()
		{
			action = new BooAction();
			context=new TestExecutionContextImpl();
		}

		[TearDown]
		public void TearDown()
		{
			action = null;
			context= null;
		}
		[Test]
		public void TestInOutEvaluation()
		{
			IDictionary config = new Hashtable();

			// config envirement (processdefinition.xml)
			config["script"]="available=available.APPROVE";
			config["available"]="InOut";
			context.SetConfiguration(config);

			// config envirement (process runtime)
			context.SetAttribute("available",Evaluation.DISAPPROVE);

			action.Run(context);

			Evaluation available = (Evaluation)context.GetAttribute("available");
			Assert.IsNotNull(available);
			Assert.IsTrue(available.Equals(Evaluation.APPROVE));
		}


		[Test]
		public void TestInOut()
		{
			IDictionary config = new Hashtable();

			// config envirement (processdefinition.xml)
			config["script"]="message=message+\"world!\"";
			config["message"]="InOut";
			context.SetConfiguration(config);

			// config envirement (process runtime)
			context.SetAttribute("message","hello ");

			action.Run(context);

			String message = (String)context.GetAttribute("message");
			Assert.IsNotNull(message);
			Assert.IsTrue(message.Equals("hello world!"));
		}

		[Test]
		public void TestOut()
		{
			IDictionary config = new Hashtable();

			// config envirement (processdefinition.xml)
			config["script"]="message=message+\"world!\"";
			config["message"]="Out";
			context.SetConfiguration(config);

			// config envirement (process runtime)
			context.SetAttribute("message","hello ");

			action.Run(context);

			String message = (String)context.GetAttribute("message");
			Assert.IsNull(message);
		}

		[Test]
		public void TestIn()
		{
			IDictionary config = new Hashtable();

			// config envirement (processdefinition.xml)
			config["script"]="message=message+\"world!\"";
			config["message"]="In";
			context.SetConfiguration(config);

			// config envirement (process runtime)
			context.SetAttribute("message","hello ");

			action.Run(context);

			String message = (String)context.GetAttribute("message");
			Assert.IsNotNull(message);
			Assert.IsTrue(message.Equals("hello "));
		}
	}
}
