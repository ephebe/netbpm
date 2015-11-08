using System;
using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
	/// <summary> 
	/// ================== 
	/// == introduction == 
	/// ================== 
	/// 
	/// This is a test for Group Assignment. Below is the hierarchy structure   
	/// of the groups involved:                                                 
	///				+=========+                                             
	///				| Group A |                                             
	///				+=========+                                             
	///					 |                                                  
	///		 ==========================                                     
	///      |                        |                                    
	/// +=========+              +=========+                              
	/// | Group B |              | Group C |                              
	/// +=========+              +=========+                              
	///		 |                                    
	/// +=========+                              
	/// | Group D |                              
	/// +=========+                              
	/// 
	/// Below are users id & name for the above group:                          
	/// 1]  uaoga ==> user A of group A                                         
	/// 2]  uboga ==> user B of group A                                         
	/// 3]  ucoga ==> user C of group A                                         
	/// 4]  uaogb ==> user A of group B                                         
	/// 5]  ubogb ==> user B of group B                                         
	/// 6]  ucogb ==> user C of group B                                         
	/// 7]  uaogc ==> user A of group C                                         
	/// 8]  ubogc ==> user B of group C                                         
	/// 9]  ucogc ==> user C of group C                                         
	/// 10] uaogd ==> user A of group D                                         
	/// 11] ubogd ==> user B of group D                                         
	/// 12] ucogd ==> user C of group D                                         
	/// 
	/// The sementics are as follows:                                           
	/// 1] when activity is assigned to group A, all members of group A, B, C, D
	/// will be able to take the activity                                       
	/// 2] when activity is assigned to group B, all members of group B only    
	/// will be able to take the activity                                       
	/// 3] when activity is assigned to group C, all members of group C, D only 
	/// will be able to take the activity                                       
	/// 4] when activity is assigned to group D, all members of group D only    
	/// will be able to take the activity                                       
	/// 
	/// </summary>
	[TestFixture]
	public class GroupAssignmentTest : AbstractExampleTest
	{
		protected override String GetParArchiv()
		{
			return "groupassignment.par";
		}

		/// <summary> This test for parent-child group assignment. Users who are members of
		/// group A, group B, group C and group D should be able to take this 
		/// task
		/// </summary>
		[Test]
		public void  TestGroupAssignmentToGroupA()
		{
			IDictionary attributeValues = CreateProcessAttributes();

			// start group assignment process instance 
			// activity assigned to group A 
			IProcessInstance processInstance = StartGroupAssignmentProcessInstance("uaoga", attributeValues);
			IList uaogaGroupTaskList = GetGroupTaskList("uaoga");
			Assert.IsFalse(uaogaGroupTaskList.Count==0);

			IList ubogaGroupTaskList = GetGroupTaskList("uboga");
			Assert.IsFalse(ubogaGroupTaskList.Count==0);
			IList ucogaGroupTaskList = GetGroupTaskList("ucoga");
			Assert.IsFalse(ucogaGroupTaskList.Count==0);
			IList uaogbGroupTaskList = GetGroupTaskList("uaogb");
			Assert.IsFalse(uaogbGroupTaskList.Count==0);
			IList ubogbGroupTaskList = GetGroupTaskList("ubogb");
			Assert.IsFalse(ubogbGroupTaskList.Count==0);
			IList ucogbGroupTaskList = GetGroupTaskList("ucogb");
			Assert.IsFalse(ucogbGroupTaskList.Count==0);
			IList uaogcGroupTaskList = GetGroupTaskList("uaogc");
			Assert.IsFalse(uaogcGroupTaskList.Count==0);
			IList ubogcGroupTaskList = GetGroupTaskList("ubogc");
			Assert.IsFalse(ubogcGroupTaskList.Count==0);
			IList ucogcGroupTaskList = GetGroupTaskList("ucogc");
			Assert.IsFalse(ucogcGroupTaskList.Count==0);
			IList uaogdGroupTaskList = GetGroupTaskList("uaogd");
			Assert.IsFalse(uaogdGroupTaskList.Count==0);
			IList ubogdGroupTaskList = GetGroupTaskList("ubogd");
			Assert.IsFalse(ubogdGroupTaskList.Count==0);
			IList ucogdGroupTaskList = GetGroupTaskList("ucogd");
			Assert.IsFalse(ucogdGroupTaskList.Count==0);
			
			CancelProcessInstance("uaoga", processInstance.Id);
		}

		/// <summary> Only users of group B would be able to take this task</summary>
		[Test]
		public void TestGroupAssignmentToGroupB()
		{
			IDictionary attributeValues = CreateProcessAttributes();
			// activity assigned to group A
			IProcessInstance processInstance = StartGroupAssignmentProcessInstance("uaogb", attributeValues);
			// simulate pull/take group assginment activity
			// activity assigned to group B
			DelegateActivity(processInstance.RootFlow.Id, "uaogb");
			testUtil.PerformActivity("uaogb", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);
		
			IList uaogaGroupTaskList = GetGroupTaskList("uaoga");
			Assert.IsTrue(uaogaGroupTaskList.Count==0);
			IList ubogaGroupTaskList = GetGroupTaskList("uboga");
			Assert.IsTrue(ubogaGroupTaskList.Count==0);
			IList ucogaGroupTaskList = GetGroupTaskList("ucoga");
			Assert.IsTrue(ucogaGroupTaskList.Count==0);
			IList uaogbGroupTaskList = GetGroupTaskList("uaogb");
			Assert.IsFalse(uaogbGroupTaskList.Count==0);
			IList ubogbGroupTaskList = GetGroupTaskList("ubogb");
			Assert.IsFalse(ubogbGroupTaskList.Count==0);
			IList ucogbGroupTaskList = GetGroupTaskList("ucogb");
			Assert.IsFalse(ucogbGroupTaskList.Count==0);
			IList uaogcGroupTaskList = GetGroupTaskList("uaogc");
			Assert.IsTrue(uaogcGroupTaskList.Count==0);
			IList ubogcGroupTaskList = GetGroupTaskList("ubogc");
			Assert.IsTrue(ubogcGroupTaskList.Count==0);
			IList ucogcGroupTaskList = GetGroupTaskList("ucogc");
			Assert.IsTrue(ucogcGroupTaskList.Count==0);
			IList uaogdGroupTaskList = GetGroupTaskList("uaogd");
			Assert.IsTrue(uaogdGroupTaskList.Count==0);
			IList ubogdGroupTaskList = GetGroupTaskList("ubogd");
			Assert.IsTrue(ubogdGroupTaskList.Count==0);
			IList ucogdGroupTaskList = GetGroupTaskList("ucogd");
			Assert.IsTrue(ucogdGroupTaskList.Count==0);
			
			CancelProcessInstance("uaogb", processInstance.Id);
		}

		/// <summary> Only users of group C would be able to take this task</summary>
		[Test]
		public void TestGroupAssignmentToGrouC()
		{
			IDictionary attributeValues = CreateProcessAttributes();
			// activity assigned to group A
			IProcessInstance processInstance = StartGroupAssignmentProcessInstance("uaogb", attributeValues);
			// simulate pull/take group assginment activity
			// activity assigned to group B
			DelegateActivity(processInstance.RootFlow.Id, "uaogb");
			testUtil.PerformActivity("uaogb", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);

			// simulate pull/take group assginment activity
			// activity assigned to group c
			DelegateActivity(processInstance.RootFlow.Id, "uaogc");
			testUtil.PerformActivity("uaogc", processInstance.RootFlow.Id, 0, attributeValues, executionComponent);

			IList uaogaGroupTaskList = GetGroupTaskList("uaoga");
			Assert.IsTrue(uaogaGroupTaskList.Count==0);
			IList ubogaGroupTaskList = GetGroupTaskList("uboga");
			Assert.IsTrue(ubogaGroupTaskList.Count==0);
			IList ucogaGroupTaskList = GetGroupTaskList("ucoga");
			Assert.IsTrue(ucogaGroupTaskList.Count==0);
			IList uaogbGroupTaskList = GetGroupTaskList("uaogb");
			Assert.IsTrue(uaogbGroupTaskList.Count==0);
			IList ubogbGroupTaskList = GetGroupTaskList("ubogb");
			Assert.IsTrue(ubogbGroupTaskList.Count==0);
			IList ucogbGroupTaskList = GetGroupTaskList("ucogb");
			Assert.IsTrue(ucogbGroupTaskList.Count==0);
			IList uaogcGroupTaskList = GetGroupTaskList("uaogc");
			Assert.IsFalse( uaogcGroupTaskList.Count==0);
			IList ubogcGroupTaskList = GetGroupTaskList("ubogc");
			Assert.IsFalse(ubogcGroupTaskList.Count==0);
			IList ucogcGroupTaskList = GetGroupTaskList("ucogc");
			Assert.IsFalse(ucogcGroupTaskList.Count==0);
			IList uaogdGroupTaskList = GetGroupTaskList("uaogd");
			Assert.IsFalse(uaogdGroupTaskList.Count==0);
			IList ubogdGroupTaskList = GetGroupTaskList("ubogd");
			Assert.IsFalse(ubogdGroupTaskList.Count==0);
			IList ucogdGroupTaskList = GetGroupTaskList("ucogd");
			Assert.IsFalse(ucogdGroupTaskList.Count==0);
			
			CancelProcessInstance("uaogc", processInstance.Id);		}

		private static IDictionary CreateProcessAttributes()
		{
			IDictionary attributeValues = new Hashtable();
			attributeValues["field A"]="field A value";
			attributeValues["field B"]="field B value";
			return attributeValues;
		}

		private IProcessInstance StartGroupAssignmentProcessInstance(string actorId, IDictionary attributeValues)
		{
			IProcessInstance processInstance = null;

			try
			{
				IProcessDefinition holidayRequest = definitionComponent.GetProcessDefinition("group assignment");

				// perform the first activity
				processInstance = executionComponent.StartProcessInstance(holidayRequest.Id, attributeValues);
				Assert.IsNotNull(processInstance);
			}
			catch (ExecutionException e)
			{
				Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
			}
			catch (Exception e)
			{
				Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
			}
			finally
			{
			}

			return processInstance;
		}

    private IList GetGroupTaskList(String actorId) {
        IList groupTaskList=null;
        IList groupIds=new ArrayList();
        try {
        	IEnumerator membershipsEnumerator=organisationComponent.FindUserById(
                actorId, new Relations("memberships.group")).Memberships.GetEnumerator();
            while(membershipsEnumerator.MoveNext()) {
                IMembership membership=(IMembership)membershipsEnumerator.Current;
                groupIds.Add(membership.Group.Id);
            }
            groupTaskList=executionComponent.GetTaskList(groupIds);
        }
        finally {
        }
        return groupTaskList;
    }

		private void CancelProcessInstance(string actorId, long processInstanceId)
		{
			executionComponent.CancelProcessInstance(processInstanceId);
		}

		private void DelegateActivity(long flowId, string actorId)
		{
			try
			{
				executionComponent.DelegateActivity(flowId, actorId);
			}
			finally
			{
			}
		}
	}
}
