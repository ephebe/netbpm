using System;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;

namespace NetBpm.Web.Presentation.Controllers
{
	public class ImageController : AbstractSecureController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (ImageController));

		public ImageController()
		{
		}

		public void ProcessImage(Int64 processDefinitionId)
		{
			IDefinitionSessionLocal definitionComponent = null;
			try
			{
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IDefinitionSessionLocal)) as IDefinitionSessionLocal;
				Context.Response.ContentType = "image/gif";
				if (log.IsDebugEnabled)
				{
					log.Debug("show ProcessImage processDefinitionId:"+processDefinitionId);
				}
				IProcessDefinition processDefinition = definitionComponent.GetProcessDefinition(processDefinitionId);
				byte[] gifContents = processDefinition.Image;
					
				if (gifContents != null)
				{
					Context.Response.OutputStream.Write(gifContents,0,gifContents.Length);
				}
			} 
			finally
			{
				ServiceLocator.Instance.Release(definitionComponent);	
			}

		}
	}
}
