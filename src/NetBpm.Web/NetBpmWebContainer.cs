using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm.Web.Controllers;

namespace NetBpm.Web
{
    public class NetBpmWebContainer : NetBpmContainer
    {
        public NetBpmWebContainer()
            : this(new XmlInterpreter("app_config.xml"))
        {
        }

        public NetBpmWebContainer(XmlInterpreter interpreter)
            : base(interpreter)
        {
            Init();
        }

        private void Init()
        {
            RegisterComponents();
        }

        private void RegisterComponents()
        {
            AddComponent("intro.controller", typeof(IntroController));
            AddComponent("user.controller", typeof(UserController));
            //AddComponent("admin.controller", typeof(AdminController));
            //AddComponent("developer.controller", typeof(DeveloperController));
            //AddComponent("form.controller", typeof(FormController));
            //AddComponent("image.controller", typeof(ImageController));
            //AddComponent("info.controller", typeof(InfoController));
        }
    }
}