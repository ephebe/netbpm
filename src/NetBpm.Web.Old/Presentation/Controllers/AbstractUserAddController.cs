namespace NetBpm.Web.Presentation.Controllers
{
	using System;
	using Castle.MonoRail.Framework;
	using NetBpm.Web.Presentation.Filters;

	[Filter(ExecuteEnum.Before, typeof(UserAddFilter) )]
	public abstract class AbstractUserAddController : AbstractController
	{
		
	}
}
