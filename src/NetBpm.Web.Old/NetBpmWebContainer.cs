using Castle.MonoRail.WindsorExtension;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm.Web.Presentation.Controllers;
using NetBpm;

namespace NetBpm.Web
{
	public class NetBpmWebContainer : NetBpmContainer
	{
		public NetBpmWebContainer() : this(new XmlInterpreter("app_config.xml"))
		{
		}

		public NetBpmWebContainer(XmlInterpreter interpreter) : base(interpreter)
		{
			Init();
		}

		private void Init()
		{
			RegisterFacilities();
			RegisterComponents();
		}

		private void RegisterFacilities()
		{
			AddFacility("rails", new RailsFacility());
		}

		private void RegisterComponents()
		{
			AddComponent("intro.controller", typeof (IntroController));
			AddComponent("user.controller", typeof (UserController));
			AddComponent("admin.controller", typeof (AdminController));
			AddComponent("developer.controller", typeof (DeveloperController));
			AddComponent("form.controller", typeof (FormController));
			AddComponent("image.controller", typeof (ImageController));
			AddComponent("info.controller", typeof (InfoController));
		}
	}
}