using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;
using NUnit.Framework;
using Castle.Windsor.Configuration.Interpreters;

namespace NetBpm.Test.Workflow.Organisation
{
	/// <summary>
	/// Zusammenfassung für OrganisationComponentTest.
	/// </summary>
	[TestFixture]
	public class OrganisationComponentTest
	{
		protected internal ServiceLocator servicelocator = null;
		protected internal IOrganisationService orgComp = null;
		private NetBpmContainer container;

		public OrganisationComponentTest()
		{
		}

		[SetUp]
		public void SetUp()
		{
			//configure the container
			container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
			servicelocator = ServiceLocator.Instance;
			orgComp = servicelocator.GetService(typeof (IOrganisationService)) as IOrganisationService;
		}

		[TearDown]
		public void TearDown()
		{
			servicelocator.Release(orgComp);
			container.Dispose();
			container = null;
		}

		[Test]
		public void TestFindActorById()
		{
			IActor actor = orgComp.FindActorById("ae");
			Assert.AreEqual("Albert Einstein", actor.Name);
		}

		[Test]
		public void TestFindUserById()
		{
			IUser user = orgComp.FindUserById("ae", new Relations("memberships"));
			Assert.AreEqual("Albert Einstein", user.Name);
			Assert.AreEqual(4, user.Memberships.Count);
		}

		[Test]
		public void TestFindGroupById()
		{
			IGroup group = orgComp.FindGroupById("group-rd", new Relations("memberships"));
			Assert.AreEqual("Research & Development", group.Name);
			Assert.AreEqual(6, group.Memberships.Count);
		}

		[Test]
		public void TestFindAllUsers()
		{
			ICollection users = orgComp.FindAllUsers();
			Assert.AreEqual(17, users.Count);
		}

		[Test]
		public void TestFindUsersByGroupAndRole()
		{
			IList users = orgComp.FindUsersByGroupAndRole("group-rd", "boss");
			IUser user = (IUser) users[0];
			Assert.AreEqual("cg", user.Id);
		}

		[Test]
		public void TestFindMembershipsByUserAndGroup()
		{
			ICollection memberships = orgComp.FindMembershipsByUserAndGroup("ed", "group-rd");
			Assert.AreEqual(2, memberships.Count);
		}

		[Test]
		public void TestFindGroupByMembership()
		{
			IGroup group = orgComp.FindGroupByMembership("ed", "hierarchy");
			Assert.AreEqual("Research & Development", group.Name);
		}
	}
}