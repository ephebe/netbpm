using System;
using System.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
	/// <summary>
	/// Test assigning attributes to NetBpm
	/// </summary>
	[TestFixture]
	public class AttributeTest : AbstractExampleTest
	{
		protected override String GetParArchiv()
		{
			return "attributetest.par";
		}

		/* ==================== */
		/* == in start-state == */
		/* ==================== */
		/// <summary> Test when extra attribute, not defined in process definition start state
		/// as field, being submitted.
		/// </summary>
		[Test]
		public void TestSubmitExtraAttributeInStartState()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] =  "field not accessible value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				attributeValues["unavailable field A"] = "unavailable field A value";
				attributeValues["unavailable field B"] = "unavailable field B value";
				StartAttributeTestProcessInstance("ae", attributeValues);
			}
			catch (System.SystemException e)
			{
				Assert.Fail("start activity of attribute test with extra attributes not defined in process definition should not get RuntimeException, instead a warn should be logged."+e.Message);
			}
		}

		/// <summary> 
		/// Test when read-only attribute, are supplied to process definition's start state.
		/// </summary>
		[Test]
		public void TestSubmitReadOnlyAttributeInStartState()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field read only"] = "field read only value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				StartAttributeTestProcessInstance("ae", attributeValues);
			}
			catch (System.ArgumentException e)
			{
				Assert.Fail("start activity of attribute test with read-only attributes passed in should not get IllegalArgumentException, instead a warn should be logged "+e.Message);
			}
		}

		/// <summary> Test when a required attribute is not suppied to process definition's start
		/// state. This should throws a RequiredFieldException, if we get that then we
		/// are safe.
		/// </summary>
		[Test]
		[Ignore("ignoring this test method for now. Castle rollback exception")]
		public void TestSubmitMissingRequiredAttributeInStartState()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field read write"] = "field read write value";
				StartAttributeTestProcessInstance("ae", attributeValues);
				Assert.Fail("RequiredFieldException should be thrown because a required attribute is missing from start state");
			}
			catch (RequiredFieldException e)
			{
				Assert.IsNull(e); // we are ok!
			}
		}

		/// <summary> Test when a not-accessible attribute is supplied to process definition's 
		/// start state. This not-accessible attribute should not be avaialble in the
		/// activity form obtained from start state.
		/// </summary>
		[Test]
		public void  TestNotAccessibleAttributeInStartState()
		{
			IProcessDefinition processDefinition = definitionComponent.GetProcessDefinition("attribute test");
			IActivityForm startActivityForm = executionComponent.GetStartForm(processDefinition.Id);
			IDictionary startActivityFormAttributeValues = startActivityForm.AttributeValues;

			//attribute values from start state activity form should not contain not accessible field attribute
			Assert.IsFalse(startActivityFormAttributeValues.Contains("field not accessible"));

			Assert.IsTrue(startActivityFormAttributeValues.Contains("field read only"));
			Assert.IsTrue(startActivityFormAttributeValues.Contains("field write only"));
			Assert.IsTrue(startActivityFormAttributeValues.Contains("field write only required"));
			Assert.IsTrue(startActivityFormAttributeValues.Contains("field read write"));
			Assert.IsTrue(startActivityFormAttributeValues.Contains("field read write required"));
		}

		/// <summary> Test when extra attribute, not defined in process definition activity state
		/// as field, being submitted.
		/// </summary>
		[Test]
		public void TestSubmitExtraAttributeInActivityState()
		{
			try
			{
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());

				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				attributeValues["unavailable field A"] = "unavailable field A value";
				attributeValues["unavailable field B"] = "unavailable field B value";

				IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);
				Assert.IsNotNull(flows);
			}
			catch (Exception e)
			{
				Assert.Fail("activity state of attribute test with extra attributes not defined in process definition should not get RuntimeException, instead a warn should be logged "+e.Message);
			}
		}

		/// <summary> 
		/// Test when read-only attribute, are supplied to process definition's activity state.
		/// </summary>
		[Test]
		public void TestSubmitReadOnlyAttributeInActivityState()
		{
			try
			{
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());

				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field read only"] = "field read only value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				
				IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);
				Assert.IsNotNull(flows);
			}
			catch (System.ArgumentException e)
			{
				Assert.Fail("activity state of attribute test with read-only attributes passed in should not get IllegalArgumentException, instead a warn should be logged"+e.Message);
			}
		}

		/// <summary> 
		/// Test when a required attribute is not suppied to process definition's activity state.
		/// </summary>
		[Test]
		[Ignore("ignoring this test method for now. Castle rollback exception")]
		public void TestSubmitMissingRequiredAttributeInActivityState()
		{
			try
			{
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());

				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field read write"] = "field read write value";
				
				testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);
				Assert.Fail("RequiredFieldException should be thrown because a required attribute is missing from activity state");
			}
			catch (RequiredFieldException e)
			{
				Assert.IsNotNull(e); //good cause RequiredFieldException is thrown
			}
		}

		/// <summary> Test when a not-accessible attribute is supplied to process definition's 
		/// activity state. This not-accessible attribute should be available in the activity
		/// form obtained from activity state
		/// </summary>
		[Test]
		public void  TestNotAccessibleAttributeInActivityState()
		{			
			IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());
			IActivityForm activityForm = executionComponent.GetActivityForm(processInstance.RootFlow.Id);

			//attribute values from activity state activity form should not contain not-accessible field attribute
			Assert.IsFalse(activityForm.AttributeValues.Contains("field not accessible"));

			/// Test when a read-only attribute is supplied to process definition's 
			/// activity state. This read-only attribute should be available in the activity
			/// form obtained from activity state
			Assert.IsTrue(activityForm.AttributeValues.Contains("field read only"));

			/// Test when a write-only attribute is supplied to process definition's
			/// activity state. This write-only attribute should be available in the activity
			/// form obtained from activity state
			Assert.IsTrue(activityForm.AttributeValues.Contains("field write only"));

			/// Test when a write-only-required attribute is supplied to process definition's
			/// activity state. This write-only-required attribute should be available in the 
			/// activity form obtained from activity state
			Assert.IsTrue(activityForm.AttributeValues.Contains("field write only required"));

			/// Test when a read-write attribute is supplied to process definition's
			/// activity state. This read-write attribute should be available in the 
			/// activity form obtained from activity state
			Assert.IsTrue(activityForm.AttributeValues.Contains("field read write"));

			/// Test when a read-write-required attribute is supplied to process definition's
			/// activity state. This read-write-required should be available in the activity form
			/// obtained from activity state
			Assert.IsTrue(activityForm.AttributeValues.Contains("field read write required"));

		}

		/// <summary> 
		/// Test when extra attribute, not defined in process definition concurrent block's 
		/// activity state as field, being submitted. 
		/// </summary>
		[Test]
		public void TestSubmitExtraAttributeInConcurrentBlock()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				attributeValues["unavailable field A"] = "unavailable field A value";
				attributeValues["unavailable field B"] = "unavailable field B value";
				
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());
				IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, CreateNormalAttributeValues(), executionComponent);
				flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 1, attributeValues, executionComponent);
				Assert.IsNotNull(flows);
			}
			catch (System.SystemException e)
			{
				Assert.Fail("concurrent block activity of attribute test with extra attributes not defined in process definition should not get RuntimeException, instead a warn should be logged "+e.Message);
			}
		}

		/// <summary> 
		/// Test when read-only attribute, are supplied to process definition's concurrent-block's activity state. 
		/// </summary>
		[Test]
		public void  TestSubmitReadOnlyAttributeInConcurrentBlock()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field read only"] = "field read only value";
				attributeValues["field write only"] = "field write only value";
				attributeValues["field write only required"] = "field write only required value";
				attributeValues["field read write"] = "field read write value";
				attributeValues["field read write required"] = "field read write required";
				
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());
				IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, CreateNormalAttributeValues(), executionComponent);
				flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 1, attributeValues, executionComponent);
				Assert.IsNotNull(flows);
			}
			catch (System.ArgumentException e)
			{
				Assert.Fail("concurrent block activity of attribute test with read-only attribute should not get IllegalArgumentException, instead a warn should be logged"+e);
			}
		}


		/// <summary> Test when a required attribute is not suppied to process definition's concurrent-block's activity
		/// state. This should throws a RequiredFieldException, if we get that then we
		/// are safe.
		/// </summary>
		[Test]
		public void TestSubmitMissingRequiredAttributeInConcurrentBlock()
		{
			try
			{
				IDictionary attributeValues = new Hashtable();
				attributeValues["field not accessible"] = "field not accessible value";
				attributeValues["field write only"] ="field write only value";
				attributeValues["field read write"] = "field read write value";
				
				IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());
				IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, CreateNormalAttributeValues(), executionComponent);
				Assert.IsNotNull(flows);
				flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 1, attributeValues, executionComponent);
				Assert.Fail("RequiredFieldException should be thrown because a required attribute is missing from concurrent block");
			}
			catch (RequiredFieldException e)
			{
				Assert.IsNotNull(e); // we are ok
			}
		}
		/// <summary> Test when a not-accessible attribute is supplied to process definition's
		/// concurrent-block's activity state. This not-accessible attribute should be available in the activity form
		/// obtained from concurrent-block's activity state
		/// </summary>
		[Test]
		public void TestNotAccessibleAttributeInConcurrentBlock()
		{
			IProcessInstance processInstance = StartAttributeTestProcessInstance("ae", CreateNormalAttributeValues());
			IList flows = testUtil.PerformActivity("ae", processInstance.RootFlow.Id, 0, CreateNormalAttributeValues(), executionComponent);
			// get the first flow (should be enough) Maybe test the second flow as well???
			IEnumerator flowEnum = flows.GetEnumerator();
			while (flowEnum.MoveNext())
			{
				IFlow flow = (IFlow)flowEnum.Current;
				IActivityForm activityForm =  executionComponent.GetActivityForm(flow.Id);
				//attribute values from concurrent block activity state activity form should not contain not-accessible field attribute
				Assert.IsFalse(activityForm.AttributeValues.Contains("field not accessible"));			

				/// Test when a read-only attribute is supplied to process definition's 
				/// concurrent-block's activity state. This read-only attribute should be available 
				/// in the activity form obtained from concurrent-block's activity state
				Assert.IsTrue(activityForm.AttributeValues.Contains("field read only"));

				/// Test when a write-only attribute is supplied to process definition's 
				/// concurrent-block's activity state. This write-only attribute should be available
				/// in the activity form obtained from concurrent-block's activity state
				Assert.IsTrue(activityForm.AttributeValues.Contains("field write only"));

				/// Test when a write-only-required attribute is supplied to process definition's 
				/// concurrent-block's activity state. This write-only-required attribute should be
				/// available in the activity form obtained from concurrent-block's activity state
				Assert.IsTrue(activityForm.AttributeValues.Contains("field write only required"));
			
				/// Test when a read-write attribute is supplied to process definition's 
				/// concurrent-block's activity state. This read-write attribute should be
				/// available in the activity form obtained form concurrent-block's activity state
				Assert.IsTrue(activityForm.AttributeValues.Contains("field read write"));

				/// Test when a read-write-required is supplied to process definition's 
				/// concurrent-block's activity state. This read-write-required attribute should be
				/// available in the activity form obtained from concurrent-block's activity state.
				Assert.IsTrue(activityForm.AttributeValues.Contains("field read write required"));
			}
		}

		private static IDictionary CreateNormalAttributeValues()
		{
			IDictionary normalAttributeValues = new Hashtable();
			normalAttributeValues["field not accessible"] = "field not accessible value";
			normalAttributeValues["field read only"] = "field read only value";
			normalAttributeValues["field write only"] = "field write only value";
			normalAttributeValues["field write only required"] = "field write only required value";
			normalAttributeValues["field read write"] = "field read write value";
			normalAttributeValues["field read write required"] = "field read write required value";
			return normalAttributeValues;
		}

		private IProcessInstance StartAttributeTestProcessInstance(String actorId, IDictionary attributeValues)
		{
			IProcessInstance processInstance = null;
			testUtil.LoginUser(actorId);

			try
			{
				IProcessDefinition processDefinition = definitionComponent.GetProcessDefinition("attribute test");

				// perform the first activity
				processInstance = executionComponent.StartProcessInstance(processDefinition.Id, attributeValues);
				Assert.IsNotNull(processInstance);
			}
			catch (ExecutionException e)
			{
				Assert.Fail("ExcecutionException while starting a new attribute test: " + e.Message);
			}
			finally
			{
				//      loginUtil.logout();
			}

			return processInstance;
		}

	}
}
