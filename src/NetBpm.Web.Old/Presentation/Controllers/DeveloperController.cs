using System.Web;
using System.Collections;
using log4net;
using Castle.MonoRail.Framework;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;

namespace NetBpm.Web.Presentation.Controllers
{
	[Layout("default")]
	public class DeveloperController : AbstractSecureController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DeveloperController));
		public DeveloperController()
		{
		}

		public void ShowHome()
		{
			log.Debug("showHome");
		}

		public void DeployProcess()
		{
			IDefinitionSessionLocal definitionComponent = null;
			try
			{
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IDefinitionSessionLocal)) as IDefinitionSessionLocal;
				IDictionary files = Context.Request.Files;
				IEnumerator enumerator = Context.Request.Files.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					log.Debug("key "+enumerator.Current);
					log.Debug("value "+files[enumerator.Current]);
					HttpPostedFile postfile = (HttpPostedFile)files[enumerator.Current];
					log.Debug("name: "+postfile.FileName);

					byte[] b = new byte[postfile.InputStream.Length];
					postfile.InputStream.Read(b, 0, (int) postfile.InputStream.Length);
					definitionComponent.DeployProcessArchive(b);
				}
			} 
			catch (NpdlException npdlEx) 
			{
				IEnumerator iter = npdlEx.ErrorMsgs.GetEnumerator();
				while (iter.MoveNext())
				{
					AddMessage(iter.Current.ToString());
				}
			}
			finally
			{
				ServiceLocator.Instance.Release(definitionComponent);
			}
			Redirect("developer","showhome");
		}	
	}
}