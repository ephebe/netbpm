using System;
using System.Collections;
using Castle.MonoRail.Framework;
using NetBpm.Workflow.Definition;

namespace NetBpm.Web.Presentation.Controllers
{
	public abstract class AbstractController: SmartDispatcherController
	{

		public AbstractController()
		{
		}

		protected void AddMessage(String message)
		{
			ArrayList messages = (ArrayList)Context.Session["messages"];
			if (messages == null)
			{
				messages = new ArrayList();
			}
			messages.Add(message);
			Context.Session.Add("messages",messages);
		}

		protected bool HasMessages()
		{
			ArrayList messages = (ArrayList)Context.Session["messages"];
			if (messages == null)
			{
				return false;
			}
			return messages.Count!=0;

		}
		/// <summary>
		/// Add the coordinates to the context 
		/// </summary>
		/// <param name="state">The State</param>
		protected void AddImageCoordinates(IState state) 
		{
			Int32[] coordinates=state.ImageCoordinates;
			if (coordinates != null)
			{
				Context.Flash["x1"]=coordinates[0];
				Context.Flash["y1"]=coordinates[1];
				Context.Flash["x2"]=coordinates[2];
				Context.Flash["y2"]=coordinates[3];
			}
		}
	}
}
