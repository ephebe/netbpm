using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Web.Controllers
{
    public class IntroController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
		public ActionResult PerformLogin(String username,String password)
		{
			if (username!=null && password!=null && username.Equals(password))
			{
				InitSession(username);

			    return Redirect("/User/ShowHome");
			} 
            else
			{
			    return View("Index");

			}
		}
	
		private void InitSession(string username) 
		{
			IPrincipal userAdapter=new PrincipalUserAdapter("ae");
			Session["user"] = userAdapter;
            HttpContext.User = userAdapter;
		}

    }
}
