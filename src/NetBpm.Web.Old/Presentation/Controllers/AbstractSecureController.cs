using System;
using Castle.MonoRail.Framework;
using NetBpm.Web.Presentation.Filters;

namespace NetBpm.Web.Presentation.Controllers
{
	[Filter(ExecuteEnum.Before, typeof(AuthenticationCheckFilter) )]
	public abstract class AbstractSecureController : AbstractController
	{
		
	}
}
